using System.Collections;
using System.Collections.Generic;
using Siccity.GLTFUtility;
using UnityEngine;

public class GltfComponentRepresentation : MonoBehaviour
{
    void Start()
    {
        UpdateVisuals(GetComponentInParent<GLTFShapeComponent>());
    }

    public void UpdateVisuals(GLTFShapeComponent gltfShape)
    {
        //Debug.Log(SceneManager.DclProjectPath + "/" + gltfShape.glbPath);
        
        Importer.LoadFromFileAsync(SceneManager.DclProjectPath + "/" + gltfShape.asset.gltfPath, new ImportSettings() { }, (
            (o, clips) =>
            {
                o.transform.SetParent(transform);
                o.transform.localPosition = Vector3.zero;
                o.transform.localScale = Vector3.one;
                o.transform.localRotation = Quaternion.identity;
                var visibleChildren = new List<Transform>();

                if (o.transform.childCount == 0)
                {
                    if (!o.name.EndsWith("_collider"))
                    {
                        visibleChildren.Add(o.transform);
                    }
                }
                else
                {
                    foreach (Transform child in o.transform)
                    {
                        if (child.name.EndsWith("_collider"))
                        {
                            child.gameObject.SetActive(false);
                        }
                        else
                        {
                            visibleChildren.Add(child);
                        }
                    }
                }



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
            }));
    }
}
