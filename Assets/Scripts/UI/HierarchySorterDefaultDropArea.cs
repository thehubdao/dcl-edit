using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchySorterDefaultDropArea : MonoBehaviour,IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag);

        if (eventData.pointerDrag.TryGetComponent<HierarchyViewItem>(out var viewItem))
        {
            viewItem.MoveLast();
        }
    }
}
