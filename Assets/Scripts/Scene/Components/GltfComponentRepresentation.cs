using System.Collections;
using System.Collections.Generic;
using Siccity.GLTFUtility;
using UnityEngine;

public class GltfComponentRepresentation : MonoBehaviour
{

    public void UpdateVisuals(GLTFShapeComponent gltfShape)
    {
        Importer.LoadFromFileAsync(SceneManager.DclProjectPath + "/" + gltfShape.glbPath, new ImportSettings() { }, (
            (o, clips) =>
            {
                o.transform.SetParent(transform);
                o.transform.localPosition = Vector3.zero;
                o.transform.localScale = Vector3.one;
                o.transform.localRotation = Quaternion.identity;
                var visibleChildren = new List<Transform>();

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

                foreach (var child in visibleChildren)
                {
                    var colliderGameObject = Instantiate(new GameObject("Collider"), o.transform);
                    colliderGameObject.layer = LayerMask.NameToLayer("Entity");
                    var newCollider = colliderGameObject.AddComponent<MeshCollider>();
                    newCollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                    child.gameObject.AddComponent<Hilightable>();
                }
            }));
    }
}
