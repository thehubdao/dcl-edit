using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.EditorState;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;

namespace Assets.Scripts.System
{
    public class ModelCacheSystem : MonoBehaviour
    {
        public static void GetModel(string path, string versionInfo, Action<GameObject> then)
        {
            if (EditorStates.CurrentModelCacheState.ModelCache.TryGetValue(path, out var value)) // TODO: check version
            {
                var instance = Instantiate(value);
                instance.SetActive(true);
                then(instance);
            }
            else
            {
                Action<GameObject> onLoadedAction = o =>
                {

                    o.transform.localPosition = Vector3.zero;
                    o.transform.localScale = Vector3.one;
                    o.transform.localRotation = Quaternion.identity;


                    var allTransforms =
                        new List<Transform>(); // all loaded transforms including all children of any level

                    var stack = new Stack<Transform>();
                    stack.Push(o.transform);
                    while (stack.Any())
                    {
                        var next = stack.Pop();
                        allTransforms.Add(next);

                        foreach (var child in next.GetChildren())
                            stack.Push(child);
                    }


                    allTransforms
                        .Where(t => t.name.EndsWith("_collider"))
                        .Where(t => t.TryGetComponent<MeshFilter>(out _))
                        .ForAll(t => t.gameObject.SetActive(false));

                    var visibleChildren = allTransforms
                        .Where(t => !t.name.EndsWith("_collider"))
                        .Where(t => t.TryGetComponent<MeshFilter>(out _));


                    // TODO: make Objects click able

                    //foreach (var child in visibleChildren)
                    //{
                    //    var colliderGameObject = Instantiate(new GameObject("Collider"), o.transform);
                    //    colliderGameObject.transform.position = child.position;
                    //    colliderGameObject.transform.rotation = child.rotation;
                    //    colliderGameObject.transform.localScale = child.localScale;
                    //
                    //    colliderGameObject.layer = LayerMask.NameToLayer("Entity");
                    //    var newCollider = colliderGameObject.AddComponent<MeshCollider>();
                    //    newCollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                    //    child.gameObject.AddComponent<Hilightable>();
                    //}
                    //
                    //DclSceneManager.OnUpdateHierarchy.Invoke();

                    if (EditorStates.CurrentModelCacheState.ModelCache.ContainsKey(path))
                        EditorStates.CurrentModelCacheState.ModelCache[path] = o;
                    else
                        EditorStates.CurrentModelCacheState.ModelCache.Add(path, o);

                    o.SetActive(false);

                    var instance = Instantiate(o);
                    instance.SetActive(true);
                    then(instance);
                };

                // Check if the model is all ready loading
                if (EditorStates.CurrentModelCacheState.CurrentlyLoading.TryGetValue(path, out var actions)) 
                {
                    // if all ready loading, just add the onLoadedAction to the list of actions to be fired
                    actions.Add(onLoadedAction);
                }
                else
                {
                    // if model is not loading yet, start loading of model
                    var onLoadedActions = new List<Action<GameObject>>();
                    onLoadedActions.Add(onLoadedAction);
                    EditorStates.CurrentModelCacheState.CurrentlyLoading.Add(path, onLoadedActions);

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

                            foreach (var loadingAction in EditorStates.CurrentModelCacheState.CurrentlyLoading[path])
                            {
                                loadingAction(o);
                            }
                        }));
                }

            }
        }
    }
}
