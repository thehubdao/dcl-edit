using System;
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
        AssetBrowserManager.OpenAssetBrowser((asset)=>
        {
            if (asset is AssetManager.GLTFAsset gltfAsset)
            {
                Spawn(entity =>
                {
                    entity.GetComponent<GLTFShapeComponent>().asset = gltfAsset;
                    entity.CustomName = gltfAsset.name;
                });
            }
            else
            {
                throw new System.Exception("Wrong Asset Type");
            }
        });
    }

    public void Spawn()
    {
        Spawn((_) => { });
    }

    private void Spawn(Action<Entity> additionalSetup)
    {
        // Instantiate entity
        var newEntityObject = Instantiate(entityPrefab, Vector3.zero, Quaternion.identity, SceneManager.EntityParent);
        var newEntity = newEntityObject.GetComponent<Entity>();

        // Setup entity
        newEntity.CustomName = defaultName;
        additionalSetup.Invoke(newEntity);
        
        // Record last selected entities for undo action
        var lastSelectedEntities = SceneManager.AllSelectedEntities.ToList();

        // Select new entity
        SceneManager.SetSelectionRaw(newEntity);
        SceneManager.ChangedHierarchy();

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
