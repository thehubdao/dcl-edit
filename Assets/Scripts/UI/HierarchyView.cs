using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HierarchyView : MonoBehaviour
{
    public GameObject itemTemplate;

    void Start()
    {
        SceneManager.OnUpdateHierarchy.AddListener(SetDirty);
        SceneManager.OnUpdateSelection.AddListener(SetDirty);
    }

    void OnEnable()
    {
        SetDirty();
    }

    private bool _dirty;
    void SetDirty()
    {
        _dirty = true;
    }

    void LateUpdate()
    {
        if (_dirty)
        {
            _dirty = false;
            UpdateVisuals();
        }
    }

    public void UpdateVisuals()
    {
        RemoveAllHierarchyItems();
        
        foreach (var entity in AllEntitiesSortedByHierarchyOrder()) 
        {
            var newItem = Instantiate(itemTemplate, transform).GetComponentInChildren<HierarchyViewItem>();
            newItem.entity = entity;
            
            newItem.UpdateVisuals();
        }
        
    }

    private void RemoveAllHierarchyItems()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private List<Entity> AllEntitiesSortedByHierarchyOrder()
    {
        var entities = SceneManager.Entities.ToList();
        // sort entities by hierarchy order
        entities.Sort(((left, right) => 
            Math.Abs(left.HierarchyOrder - right.HierarchyOrder) < 0.0000001?0:
            left.HierarchyOrder < right.HierarchyOrder?-1:1));

        return entities;
    }
}
