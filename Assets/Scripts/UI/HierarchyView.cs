using System.Collections;
using System.Collections.Generic;
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
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        int i = 0;
        foreach (var entity in SceneManager.Entities)
        {
            var newItem = Instantiate(itemTemplate, transform).GetComponentInChildren<HierarchyViewItem>();
            newItem.entity = entity;

            var itemRectTransform = newItem.GetComponent<RectTransform>();
            itemRectTransform.Translate(0, i++ * -20f, 0);// = new Vector3();

            newItem.UpdateVisuals();
        }

        var rectTransform = GetComponent<RectTransform>();
        var newSize = rectTransform.sizeDelta;
        newSize.y = i * 20;
        rectTransform.sizeDelta = newSize;
    }
}
