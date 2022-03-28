using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollapseArrowUI : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    private HierarchyViewItem viewItem;

    

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click Collapse");

        viewItem.entity.CollapsedChildren = !viewItem.entity.CollapsedChildren;
    }
}
