using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityItem : MonoBehaviour
{
    public GameObject entityPrefab;
    public Transform entityParent;

    public void Spawn()
    {
        Instantiate(entityPrefab, Vector3.zero, Quaternion.identity, entityParent);
        SceneManager.ChangedHierarchy();
    }
}
