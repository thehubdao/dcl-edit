using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchySorterDropArea : MonoBehaviour,IDropHandler
{
    public enum DropPlace
    {
        Before,
        Child,
        After
    }
    
    [SerializeField]
    public HierarchyViewItem ownViewItem;

    [SerializeField]
    public DropPlace place;


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag);

        if (eventData.pointerDrag.TryGetComponent<HierarchyViewItem>(out var viewItem))
        {
            switch (place)
            {
                case DropPlace.Before:
                    viewItem.MoveBefore(ownViewItem.entity);
                    break;
                case DropPlace.Child:
                    viewItem.MoveToChild(ownViewItem.entity);
                    break;
                case DropPlace.After:
                    viewItem.MoveAfter(ownViewItem.entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
