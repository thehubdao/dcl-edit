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

    public void Spawn()
    {
        var newEntityObject = Instantiate(entityPrefab, Vector3.zero, Quaternion.identity, SceneManager.EntityParent);

        if (glbFileName != "")
        {
            newEntityObject.GetComponent<GLTFShapeComponent>().glbPath = glbFileName;
        }

        SceneManager.ChangedHierarchy();
        var newEntity = newEntityObject.GetComponent<Entity>();
        newEntity.customName = defaultName;
        SceneManager.SelectedEntity = newEntity;
    }
}
