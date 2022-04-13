using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;
using ImportOptions = UnityGLTF.ImportOptions;


public class GltfComponentRepresentation : MonoBehaviour
{
    void Start()
    {
        UpdateVisuals(GetComponentInParent<GLTFShapeComponent>());
        DclSceneManager.OnUpdateSelection.AddListener(() =>
        {
            var gltfShapeComponent = GetComponentInParent<GLTFShapeComponent>();
            if (gltfShapeComponent != null)
                UpdateVisuals(gltfShapeComponent);
        });
    }

    private AssetManager.Asset _shownAsset = AssetManager.Asset.emptyAsset;

    public void UpdateVisuals(GLTFShapeComponent gltfShape)
    {
        //Debug.Log(SceneManager.DclProjectPath + "/" + gltfShape.glbPath);

        if (_shownAsset != gltfShape.asset)
        {
            //Debug.Log("Updating GLTF Component representation");

            _shownAsset = gltfShape.asset;
            try
            {
                if (gltfShape.asset == null)
                    throw new IOException("No asset selected");

                var filePath = DclSceneManager.DclProjectPath + "/" + gltfShape.asset.gltfPath;
                var filePathParts = filePath.Split('/', '\\');

                var options = new ImportOptions()
                {
                    DataLoader = new FileLoader(URIHelper.GetDirectoryName(filePath)),
                    AsyncCoroutineHelper = gameObject.AddComponent<AsyncCoroutineHelper>()
                };
                
                var importer = new GLTFSceneImporter(filePathParts[filePathParts.Length - 1], options);

                importer.CustomShaderName = "Shader Graphs/GLTFShader";

                StartCoroutine(importer.LoadScene(
                    onLoadComplete: (loadedObject, info) =>
                    {
                        if(loadedObject == null)
                        {
                            Debug.LogError(info.SourceException.Message+"\n"+info.SourceException.StackTrace);
                            return;
                        }

                        foreach (Transform child in transform)
                        {
                            Destroy(child.gameObject);
                        }

                        loadedObject.transform.SetParent(transform);
                        loadedObject.transform.localPosition = Vector3.zero;
                        loadedObject.transform.localScale = Vector3.one;
                        loadedObject.transform.localRotation = Quaternion.identity;


                        var allTransforms =
                            new List<Transform>(); // all loaded transforms including all children of any level

                        var stack = new Stack<Transform>();
                        stack.Push(loadedObject.transform);
                        while (stack.Any())
                        {
                            var next = stack.Pop();
                            allTransforms.Add(next);

                            foreach (var child in next.Children())
                                stack.Push(child);
                        }


                        allTransforms
                            .Where(t => t.name.EndsWith("_collider"))
                            .Where(t => t.TryGetComponent<MeshFilter>(out _))
                            .Forall(t => t.gameObject.SetActive(false));

                        var visibleChildren = allTransforms
                            .Where(t => !t.name.EndsWith("_collider"))
                            .Where(t => t.TryGetComponent<MeshFilter>(out _)||t.TryGetComponent<SkinnedMeshRenderer>(out _));


                        foreach (var child in visibleChildren)
                        {
                            var colliderGameObject = new GameObject("Collider");
                            colliderGameObject.transform.SetParent(loadedObject.transform);
                            colliderGameObject.transform.position = child.position;
                            colliderGameObject.transform.rotation = child.rotation;
                            colliderGameObject.transform.localScale = child.localScale;

                            colliderGameObject.layer = LayerMask.NameToLayer("Entity");
                            var newCollider = colliderGameObject.AddComponent<MeshCollider>();
                            
                            if(child.TryGetComponent(out MeshFilter meshFilter))
                                newCollider.sharedMesh = meshFilter.sharedMesh;
                            else if (child.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
                                newCollider.sharedMesh = skinnedMeshRenderer.sharedMesh;
                            else
                                throw new Exception("Could not find mesh filter or skinned mesh renderer");
                            
                            child.gameObject.AddComponent<Hilightable>();
                        }

                        DclSceneManager.OnUpdateHierarchy.Invoke();

                    }));
                
            }
            catch (IOException e)
            {
                Debug.LogWarning(e.Message);

                var errorObject = Instantiate(AssetAssetsList.ErrorModel);

                errorObject.transform.SetParent(transform);
                errorObject.transform.localPosition = Vector3.zero;
                errorObject.transform.localScale = Vector3.one;
                errorObject.transform.localRotation = Quaternion.identity;
            }
        }

    }
}
