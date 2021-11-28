using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class EntityItem : MonoBehaviour
{
    public GameObject entityPrefab;
    
    public TextMeshProUGUI text;

    public string glbFileName = "";

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

        if (gltfAsset != null)
        {
            newEntityObject.GetComponent<GLTFShapeComponent>().asset = gltfAsset;
        }

        SceneManager.ChangedHierarchy();
        var newEntity = newEntityObject.GetComponent<Entity>();
        newEntity.CustomName = defaultName;
        SceneManager.SelectedEntity = newEntity;
    }
}
