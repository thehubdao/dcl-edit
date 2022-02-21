using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Siccity.GLTFUtility;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;


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
            Debug.Log("Updating GLTF Component representation");

            _shownAsset = gltfShape.asset;
            try
            {
                if (gltfShape.asset == null)
                    throw new IOException("No asset selected");

                var filePath = DclSceneManager.DclProjectPath + "/" + gltfShape.asset.gltfPath;

                

                Importer.LoadFromFileAsync(
                    DclSceneManager.DclProjectPath + "/" + gltfShape.asset.gltfPath,
                    new ImportSettings() { },
                    (o, clips) =>
                    {
                        foreach (Transform child in transform)
                        {
                            Destroy(child.gameObject);
                        }

                        o.transform.SetParent(transform);
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

                            foreach (var child in next.Children())
                                stack.Push(child);
                        }


                        allTransforms
                            .Where(t => t.name.EndsWith("_collider"))
                            .Where(t => t.TryGetComponent<MeshFilter>(out _))
                            .Forall(t => t.gameObject.SetActive(false));

                        var visibleChildren = allTransforms
                            .Where(t => !t.name.EndsWith("_collider"))
                            .Where(t => t.TryGetComponent<MeshFilter>(out _));


                        foreach (var child in visibleChildren)
                        {
                            var colliderGameObject = Instantiate(new GameObject("Collider"), o.transform);
                            colliderGameObject.transform.position = child.position;
                            colliderGameObject.transform.rotation = child.rotation;
                            colliderGameObject.transform.localScale = child.localScale;

                            colliderGameObject.layer = LayerMask.NameToLayer("Entity");
                            var newCollider = colliderGameObject.AddComponent<MeshCollider>();
                            newCollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                            child.gameObject.AddComponent<Hilightable>();
                        }

                        DclSceneManager.OnUpdateHierarchy.Invoke();
                    });
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
