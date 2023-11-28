using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;
using Zenject;

namespace Assets.Scripts.System
{
    public class LoadGltfFromFileSystem
    {
        public void LoadGltfFromPath(string gltfPath, Action<GameObject> then, IDataLoader dataLoader = null)
        {
            try
            {
                dataLoader ??= new FileLoader(URIHelper.GetDirectoryName(gltfPath));

                var options = new ImportOptions()
                {
                    DataLoader = dataLoader,
                    AsyncCoroutineHelper = GameObject.Find("EditorStates").GetComponent<AsyncCoroutineHelper>()
                };

                var importer = new GLTFSceneImporter(gltfPath, options);
                importer.CustomShaderName = "Shader Graphs/GLTFShader";

                importer.LoadSceneAsync(
                    onLoadComplete: (o, info) =>
                    {
                        if (o == null)
                        {
                            Debug.LogError(info.SourceException.Message + "\n" + info.SourceException.StackTrace);
                            then(null);

                            return;
                        }

                        // Prepare loaded model
                        // reset transform
                        o.transform.localPosition = Vector3.zero;
                        o.transform.localScale = Vector3.one;
                        o.transform.localRotation = Quaternion.identity;

                        var allTransforms = new List<Transform>(); // All loaded transforms including all children of any level

                        // Fill the allTransforms List
                        var stack = new Stack<Transform>();
                        stack.Push(o.transform);
                        while (stack.Any())
                        {
                            var next = stack.Pop();
                            allTransforms.Add(next);

                            foreach (var child in next.GetChildren())
                                stack.Push(child);
                        }

                        // Make all decentraland collider invisible
                        allTransforms
                            .Where(t => t.name.EndsWith("_collider"))
                            .Where(t => t.TryGetComponent<MeshFilter>(out _))
                            .ForAll(t => t.gameObject.SetActive(false));

                        // Find all transforms of visible GameObjects
                        var visibleChildren = allTransforms
                            .Where(t => !t.name.EndsWith("_collider"))
                            .Where(t => t.TryGetComponent<MeshFilter>(out _) || t.TryGetComponent<SkinnedMeshRenderer>(out _));

                        //Change opaque shader to transparent shader when necessary
                        var transforms = visibleChildren.ToList();
                        foreach (var transform in transforms)
                        {
                            var materials = transform.GetComponent<Renderer>().materials;
                            
                            foreach (var material in materials)
                            {
                                if (material.color.a < 1)
                                {
                                    material.shader = Shader.Find("Shader Graphs/GLTFShaderTrans");
                                }
                            }
                        }

                        // Add click collider to all visible GameObjects
                        foreach (var child in transforms)
                        {
                            var colliderGameObject = new GameObject("Click Collider");
                            colliderGameObject.transform.parent = child;
                            colliderGameObject.transform.localPosition = Vector3.zero;
                            colliderGameObject.transform.localRotation = Quaternion.identity;
                            colliderGameObject.transform.localScale = Vector3.one;

                            colliderGameObject.layer = 10; // Entity Click Layer
                            var newCollider = colliderGameObject.AddComponent<MeshCollider>();

                            if (child.TryGetComponent<MeshFilter>(out var meshFilter))
                                newCollider.sharedMesh = meshFilter.sharedMesh;

                            if (child.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
                            {
                                Mesh bakedMesh = new Mesh();
                                skinnedMeshRenderer.BakeMesh(bakedMesh, true);
                                newCollider.sharedMesh = bakedMesh;
                            }
                        }

                        // Add the loaded model to cache. Copies of it will be created when the cache is used.
                        GameObject parent = GameObject.Find("ModelCache") ?? new GameObject("ModelCache");
                        parent.transform.position = new Vector3(0, -5000, 0); // Place out of sight to avoid the user seeing objects get instantiated
                        o.transform.SetParent(parent.transform);
                        o.SetActive(false);

                        then(o);
                    });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
