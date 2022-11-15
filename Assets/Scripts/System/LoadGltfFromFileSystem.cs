using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;
using Zenject;

namespace Assets.Scripts.System
{
    public class LoadGltfFromFileSystem
    {
        // Dependencies
        private UnityState _unityState;

        [Inject]
        public void Construct(UnityState unityState)
        {
            _unityState = unityState;
        }

        public void LoadGltfFromPath(string gltfPath, Action<GameObject> then)
        {
            try
            {
                var options = new ImportOptions()
                {
                    DataLoader = new FileLoader(URIHelper.GetDirectoryName(gltfPath)),
                    AsyncCoroutineHelper = _unityState.AsyncCoroutineHelper
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


                        // Add click collider to all visible GameObjects
                        foreach (var child in visibleChildren)
                        {
                            var colliderGameObject = new GameObject("Collider");
                            colliderGameObject.transform.parent = o.transform;
                            colliderGameObject.transform.position = child.position;
                            colliderGameObject.transform.rotation = child.rotation;
                            colliderGameObject.transform.localScale = child.localScale;

                            colliderGameObject.layer = 10; // Entity Click Layer
                            var newCollider = colliderGameObject.AddComponent<MeshCollider>();

                            if (child.TryGetComponent<MeshFilter>(out var meshFilter))
                                newCollider.sharedMesh = meshFilter.sharedMesh;

                            if (child.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
                                newCollider.sharedMesh = skinnedMeshRenderer.sharedMesh;
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