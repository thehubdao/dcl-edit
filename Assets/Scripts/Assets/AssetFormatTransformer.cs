using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Assets
{
    public class AssetFormatTransformer
    {
        public abstract class AssetTransformation
        {
            public abstract void Transform();
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
                public AssetTransformation transformation;
            }

            public List<Node> nodes = new();
            public List<Edge> edges = new();

            private void AddNode(Type formatType)
            {
                var node = new Node
                {
                    formatType = formatType
                };
                nodes.Add(node);
            }

            private void AddEdge(Node to, Node from, AssetTransformation transformation)
            {
                var edge = new Edge
                {
                    from = from,
                    to = to,
                    transformation = transformation
                };
                edges.Add(edge);
            }

            private Node GetNodeOrNull(Type formatType)
            {
                return nodes.Find(n => n.formatType == formatType);
            }

            public void Add(Type fromType, Type toType, AssetTransformation transformation)
            {
                Assert.IsTrue(fromType.IsSubclassOf(typeof(CommonAssetTypes.AssetFormat)));
                Assert.IsTrue(toType.IsSubclassOf(typeof(CommonAssetTypes.AssetFormat)));

                var from = GetNodeOrNull(fromType);
                if (from == null)
                {
                    AddNode(fromType);
                    from = nodes.Find(n => n.formatType == fromType);
                }

                var to = GetNodeOrNull(toType);
                if (to == null)
                {
                    AddNode(toType);
                    to = nodes.Find(n => n.formatType == toType);
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

            public AssetTransformation GetNextStep(Type fromType, Type toType)
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


        public void Init()
        {
            var graph = new FormatsGraph();

            graph.Add(typeof(AssetFormatBuilderCloud), typeof(AssetFormatBuilderDownload), new TransformerBuilderCloudToBuilderDownload());

            graph.CalculatePaths();
        }
    }
}