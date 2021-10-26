using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyView : MonoBehaviour
{
    public GameObject itemTemplate;

    void Update()
    {
        //UpdateVisuals(); // TODO: Make a smart decision, when to update visuals
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
            var newItem = Instantiate(itemTemplate, transform).GetComponent<HierarchyViewItem>();
            newItem.name = entity.name;

            newItem.GetComponent<RectTransform>().Translate(0, i++ * -50f, 0);// = new Vector3();

            newItem.UpdateVisuals();
        }

        var rectTransform = GetComponent<RectTransform>();
        var newSize = rectTransform.sizeDelta;
        newSize.y = i * 50;
        rectTransform.sizeDelta = newSize;
    }
}
