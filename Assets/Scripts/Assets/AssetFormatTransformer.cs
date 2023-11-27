using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Assets.Scripts.Assets
{
    public class AssetFormatTransformer
    {
        // Dependencies
        private IAssetTransformation[] assetTransformations;

        [Inject]
        private void Construct(IAssetTransformation[] assetTransformations)
        {
            this.assetTransformations = assetTransformations;
        }


        public interface IAssetTransformation
        {
            public Type fromType { get; }
            public Type toType { get; }
            public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset);
        }


        class FormatsGraph
        {
            public class Node
            {
                public Type formatType;

                public Dictionary<Node, Edge> nextEdges = new();
            }

            public class Edge
            {
                public Node from;
                public Node to;
                public IAssetTransformation transformation;
            }

            private readonly List<Node> nodes = new();
            private readonly List<Edge> edges = new();

            private void AddNode(Type formatType)
            {
                var node = new Node
                {
                    formatType = formatType
                };
                nodes.Add(node);
            }

            private void AddEdge(Node to, Node from, IAssetTransformation transformation)
            {
                var edge = new Edge
                {
                    from = from,
                    to = to,
                    transformation = transformation
                };
                edges.Add(edge);
            }

            //public Type GetFromType(IAssetTransformation assetTransformation)
            //{
            //    return edges.First(e => e.transformation == assetTransformation).from.formatType;
            //}
            //
            //public Type GetToType(IAssetTransformation assetTransformation)
            //{
            //    return edges.First(e => e.transformation == assetTransformation).to.formatType;
            //}

            private Node GetNodeOrNull(Type formatType)
            {
                return nodes.Find(n => n.formatType == formatType);
            }

            public void Add(IAssetTransformation transformation)
            {
                Assert.IsTrue(transformation.fromType.IsSubclassOf(typeof(CommonAssetTypes.AssetFormat)));
                Assert.IsTrue(transformation.toType.IsSubclassOf(typeof(CommonAssetTypes.AssetFormat)));

                var from = GetNodeOrNull(transformation.fromType);
                if (from == null)
                {
                    AddNode(transformation.fromType);
                    from = nodes.Find(n => n.formatType == transformation.fromType);
                }

                var to = GetNodeOrNull(transformation.toType);
                if (to == null)
                {
                    AddNode(transformation.toType);
                    to = nodes.Find(n => n.formatType == transformation.toType);
                }

                AddEdge(to, from, transformation);
            }

            public void CalculatePaths()
            {
                // clear all next edges
                foreach (var node in nodes)
                {
                    node.nextEdges.Clear();
                }

                // for each node 
                foreach (var node in nodes)
                {
                    // all edges that start from this node
                    var edges = this.edges.FindAll(e => e.from == node);

                    // distance to each node
                    var distances = new Dictionary<Node, int>();

                    // for each edge
                    foreach (var edge in edges)
                    {
                        var reachableNodes = new Dictionary<Node, int>();

                        // find all nodes behind this edge
                        FindAllReachableNodes(edge.to, reachableNodes);

                        // for each reachable node add to distances
                        foreach (var (reachableNode, reachableDistance) in reachableNodes)
                        {
                            // see if we already have a distance to this node
                            if (distances.TryGetValue(reachableNode, out var currentDistance))
                            {
                                // if we have a distance, the new distance is shorter than the current one
                                if (reachableDistance < currentDistance)
                                {
                                    // if it is shorter, replace it
                                    node.nextEdges[reachableNode] = edge;
                                    distances[reachableNode] = reachableDistance;
                                }
                            }
                            else
                            {
                                // if we don't have a distance, add it
                                node.nextEdges.Add(reachableNode, edge);
                                distances[reachableNode] = reachableDistance;
                            }
                        }
                    }
                }
            }

            private void FindAllReachableNodes(Node node, Dictionary<Node, int> result, int distance = 0)
            {
                // if we already have this node, check if the distance is shorter
                if (result.TryGetValue(node, out var currentDistance))
                {
                    // if it is shorter, replace it
                    if (distance >= currentDistance) return;

                    result[node] = distance;
                }
                else
                {
                    // if we don't have it, add it
                    result.Add(node, distance);
                }

                // find all nodes behind this node
                var newEdges = this.edges.FindAll(e => e.from == node);

                // for each edge
                foreach (var edge in newEdges)
                {
                    FindAllReachableNodes(edge.to, result, distance + 1);
                }
            }

            public IAssetTransformation GetNextStep(Type fromType, Type toType)
            {
                var from = GetNodeOrNull(fromType);
                var to = GetNodeOrNull(toType);
                if (from == null || to == null)
                {
                    return null;
                }

                if (from.nextEdges.TryGetValue(to, out var edge))
                {
                    return edge.transformation;
                }

                return null;
            }
        }


        private FormatsGraph formatsGraph = new();

        public void Init()
        {
            foreach (var transformation in assetTransformations)
            {
                formatsGraph.Add(transformation);
            }

            formatsGraph.CalculatePaths();
        }


        public enum TransformToFormatReturn
        {
            Available,
            Loading,
            Impossible
        }

        public TransformToFormatReturn TransformToFormat(CommonAssetTypes.AssetInfo asset, Type toFormat)
        {
            // check if already done
            if (asset.availableFormats.Any(f => f.GetType() == toFormat))
            {
                var searchedFormat = asset.availableFormats.First(f => f.GetType() == toFormat);
                if (searchedFormat.availability == CommonAssetTypes.Availability.Loading)
                    return TransformToFormatReturn.Loading;
                return TransformToFormatReturn.Available;
            }

            // get starting format
            var baseFormatType = asset.baseFormat.GetType();

            // check if impossible
            var nextStep = formatsGraph.GetNextStep(baseFormatType, toFormat);
            if (nextStep == null) return TransformToFormatReturn.Impossible;

            var nextFormat = nextStep.toType;

            while (asset.GetAssetFormatOrNull(nextFormat) != null)
            {
                nextStep = formatsGraph.GetNextStep(nextFormat, toFormat);
                nextFormat = nextStep.toType;
            }

            var currentFormat = nextStep.fromType;

            var newFormat = nextStep.Transform(asset.GetAssetFormatOrNull(currentFormat), asset);

            asset.availableFormats.Add(newFormat);

            return newFormat.availability == CommonAssetTypes.Availability.Available ?
                TransformToFormat(asset, toFormat) : // If instantly available, transform further
                TransformToFormatReturn.Loading; // If format is loading, return loading
        }
    }
}