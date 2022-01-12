using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HierarchyView : MonoBehaviour
{
    [SerializeField]
    private GameObject itemTemplate;

    [SerializeField]
    private GameObject dropIndicatorTemplate;



    [NonSerialized]
    public DropIndicatorUI dropIndicator;


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
        
        //foreach (var entity in AllEntitiesSortedByHierarchyOrder()) 
        //{
        //    var newItem = Instantiate(itemTemplate, transform).GetComponentInChildren<HierarchyViewItem>();
        //    newItem.entity = entity;
        //    
        //    newItem.UpdateVisuals();
        //}

        dropIndicator = Instantiate(dropIndicatorTemplate, transform).GetComponent<DropIndicatorUI>();
        dropIndicator.HideIndicator();
        
        AddItemsRecursive(SceneManager.SceneRoot);
    }

    private void AddItemsRecursive(SceneTreeObject sto)
    {
        foreach (var child in sto.Children) 
        {
            var newItem = Instantiate(itemTemplate, transform).GetComponentInChildren<HierarchyViewItem>();
            newItem.entity = child as Entity;
            
            newItem.UpdateVisuals();

            AddItemsRecursive(child);
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
        entities.Sort((left, right) => left.HierarchyOrder.CompareTo(right.HierarchyOrder));

        return entities;
    }

    public void EnableDropZones()
    {
        SetDropZonesActive(true);
    }

    public void DisableDropZones()
    {
        SetDropZonesActive(false);
    }

    private void SetDropZonesActive(bool value)
    {
        var items = GetComponentsInChildren<HierarchyViewItem>();
        foreach (var item in items)
        {
            item.SetDropZoneActive(value);
        }
    }
}
