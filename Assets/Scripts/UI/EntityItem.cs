using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class EntityItem : MonoBehaviour
{
    public GameObject entityPrefab;
    
    public TextMeshProUGUI text;
    
    public string defaultName = "";

    public void GltfSpawn()
    {
        AssetBrowserManager.OpenAssetBrowser((asset)=> {
            if (asset.GetType() != typeof(AssetManager.GLTFAsset))
            {
                throw new System.Exception("Wrong Asset Type");
            }
            Spawn((AssetManager.GLTFAsset)asset);
        });
    }

    public void Spawn()
    {
        Spawn(null);
    }

    public void Spawn(AssetManager.GLTFAsset gltfAsset)
    {
        var newEntityObject = Instantiate(entityPrefab, Vector3.zero, Quaternion.identity, SceneManager.EntityParent);

        var newEntity = newEntityObject.GetComponent<Entity>();

        if (gltfAsset != null)
        {
            newEntityObject.GetComponent<GLTFShapeComponent>().asset = gltfAsset;
            newEntity.CustomName = gltfAsset.name;
        }
        else
        {
            newEntity.CustomName = defaultName;
        }


        var lastSelectedEntities = SceneManager.AllSelectedEntities.ToList();
        SceneManager.ChangedHierarchy();
        SceneManager.SetSelectionRaw(newEntity);

        // Undo stuff

        UndoManager.RecordUndoItem("Added Entity "+newEntity.CustomName,
            () =>
            {
                TrashBinManager.DeleteEntity(newEntity);
                SceneManager.ChangedHierarchy();
                SceneManager.SetSelectionRaw(null);

                foreach (var entity in lastSelectedEntities)
                {
                    SceneManager.AddSelectedRaw(entity);
                }
            },
            () =>
            {
                TrashBinManager.RestoreEntity(newEntity);
                SceneManager.ChangedHierarchy();
                SceneManager.SetSelectionRaw(newEntity);
            });
    }
}
