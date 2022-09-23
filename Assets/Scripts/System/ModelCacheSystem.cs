using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.EditorState;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;
using Zenject;

namespace Assets.Scripts.System
{
    public class ModelCacheSystem
    {
        // Dependencies
        private ModelCacheState _modelCacheState;

        [Inject]
        private void Construct(ModelCacheState modelCacheState)
        {
            _modelCacheState = modelCacheState;
        }

        /// <summary>
        /// Gets the cached model for the specified path
        /// </summary>
        /// <param name="path">the path of the model</param>
        /// <param name="versionInfo">a specifier for the version of the object. Can be any string. E.g.: version counter, timestamp, hash, ...</param>
        /// <param name="then">An action that is called with the Specified Model as a GameObject. This might get called to a later point, if the Model needs to be loaded first.</param>
        public void GetModel(string path, string versionInfo, Action<GameObject> then)
        {
            if (_modelCacheState.ModelCache.TryGetValue(path, out var value)) // TODO: check version
            {
                var instance = GameObject.Instantiate(value);
                instance.SetActive(true);
                then(instance);
            }
            else
            {
                Action<GameObject> onLoadedAction = o =>
                {
                    var instance = GameObject.Instantiate(o);
                    instance.SetActive(true);
                    then(instance);
                };

                // Check if the model is already loading
                if (_modelCacheState.CurrentlyLoading.TryGetValue(path, out var actions))
                {
                    // if already loading, just add the onLoadedAction to the list of actions to be fired
                    actions.Add(onLoadedAction);
                }
                else
                {
                    // if model is not loading yet, start loading of model
                    var onLoadedActions = new List<Action<GameObject>>();
                    onLoadedActions.Add(onLoadedAction);
                    _modelCacheState.CurrentlyLoading.Add(path, onLoadedActions);

                    var absolutePath = EditorStates.CurrentPathState.ProjectPath + "/" + path;

                    var filePathParts = absolutePath.Split('/', '\\');

                    var options = new ImportOptions()
                    {
                        DataLoader = new FileLoader(URIHelper.GetDirectoryName(absolutePath)),
                        AsyncCoroutineHelper = EditorStates.CurrentUnityState.AsyncCoroutineHelper
                    };

                    var importer = new GLTFSceneImporter(filePathParts[filePathParts.Length - 1], options);

                    importer.CustomShaderName = "Shader Graphs/GLTFShader";

                    EditorStates.CurrentUnityState.AsyncCoroutineHelper.StartCoroutine(importer.LoadScene(
                        onLoadComplete: (o, info) =>
                        {
                            if (o == null)
                            {
                                Debug.LogError(info.SourceException.Message + "\n" + info.SourceException.StackTrace);
                                return;
                            }

                            // prepare loaded model

                            // reset transform
                            o.transform.localPosition = Vector3.zero;
                            o.transform.localScale = Vector3.one;
                            o.transform.localRotation = Quaternion.identity;


                            var allTransforms =
                                new List<Transform>(); // all loaded transforms including all children of any level

                            // fill the allTransforms List
                            var stack = new Stack<Transform>();
                            stack.Push(o.transform);
                            while (stack.Any())
                            {
                                var next = stack.Pop();
                                allTransforms.Add(next);

                                foreach (var child in next.GetChildren())
                                    stack.Push(child);
                            }

                            // make all decentraland collider invisible
                            allTransforms
                                .Where(t => t.name.EndsWith("_collider"))
                                .Where(t => t.TryGetComponent<MeshFilter>(out _))
                                .ForAll(t => t.gameObject.SetActive(false));

                            // find all transforms of visible GameObjects
                            var visibleChildren = allTransforms
                                .Where(t => !t.name.EndsWith("_collider"))
                                .Where(t => t.TryGetComponent<MeshFilter>(out _) || t.TryGetComponent<SkinnedMeshRenderer>(out _));


                            // add click collider to all visible GameObjects
                            foreach (var child in visibleChildren)
                            {
                                var colliderGameObject = GameObject.Instantiate(new GameObject("Collider"), o.transform);
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

                            if (_modelCacheState.ModelCache.ContainsKey(path))
                                _modelCacheState.ModelCache[path] = o;
                            else
                                _modelCacheState.ModelCache.Add(path, o);

                            o.SetActive(false);

                            // dispatch all actions for the loading complete event
                            foreach (var loadingCompleteAction in _modelCacheState.CurrentlyLoading[path])
                            {
                                loadingCompleteAction(o);
                            }
                        }));
                }
            }
        }
    }
}
