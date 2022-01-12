using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DropIndicatorUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform _thisRectTransform;

    [SerializeField]
    private RectTransform _lineRectTransform;



    public void SetIndicatorToItem(HierarchyViewItem item, HierarchySorterDropArea.DropPlace place)
    {
        Debug.Log("set indicator");
        
        _lineRectTransform.gameObject.SetActive(true);
        

        switch (place)
        {
            case HierarchySorterDropArea.DropPlace.Before:
                _thisRectTransform.anchoredPosition = 
                    item.transform.parent.GetComponent<RectTransform>().anchoredPosition;

                _lineRectTransform.SetLeft((item.entity.Level+1) * 20);

                break;

            case HierarchySorterDropArea.DropPlace.After:
                _thisRectTransform.anchoredPosition = 
                    item.transform.parent.GetComponent<RectTransform>().anchoredPosition 
                    - new Vector2(0,30);

                _lineRectTransform.SetLeft((item.entity.Level+1) * 20);

                break;

            case HierarchySorterDropArea.DropPlace.Child:
                _thisRectTransform.anchoredPosition = 
                    item.transform.parent.GetComponent<RectTransform>().anchoredPosition 
                    - new Vector2(0, (item.entity.ChildCount + 1)*30);

                _lineRectTransform.SetLeft((item.entity.Level + 2) * 20);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(place), place, null);
        }
        
    }

    public void HideIndicator()
    {
        Debug.Log("hide indicator");
        
        _lineRectTransform.gameObject.SetActive(false);
    }
}
