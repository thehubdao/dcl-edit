using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ContextMenuHoverHandler : MonoBehaviour, IPointerEnterHandler
{
    public UnityAction OnHoverAction;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverAction?.Invoke();
    }
}
