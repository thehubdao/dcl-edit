using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchySorterDropArea : MonoBehaviour,IDropHandler
{
    enum DropPlace
    {
        Before,
        Child,
        After
    }
    
    [SerializeField]
    private HierarchyViewItem _ownViewItem;

    [SerializeField]
    private DropPlace place;


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag);

        if (eventData.pointerDrag.TryGetComponent<HierarchyViewItem>(out var viewItem))
        {
            switch (place)
            {
                case DropPlace.Before:
                    viewItem.MoveBefore(_ownViewItem.entity);
                    break;
                case DropPlace.Child:
                    viewItem.MoveToChild(_ownViewItem.entity);
                    break;
                case DropPlace.After:
                    viewItem.MoveAfter(_ownViewItem.entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
