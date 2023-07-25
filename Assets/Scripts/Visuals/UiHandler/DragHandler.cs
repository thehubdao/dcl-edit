using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [CanBeNull]
    private DragStrategy dragStrategyInternal;

    // Dependencies
    private DragAndDropState dragAndDropState;

    [CanBeNull]
    public DragStrategy dragStrategy
    {
        get => dragStrategyInternal;
        set
        {
            enabled = value != null; // disable drag handler when the strategy is null
            dragStrategyInternal = value;
        }
    }

    [Inject]
    private void Construct(DragAndDropState dragAndDropState)
    {
        this.dragAndDropState = dragAndDropState;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // enable all corresponding drop zones
        //Debug.Log($"Begin Drag of {StaticUtilities.ListGameObjectStack(gameObject)}");
        dragAndDropState.BeginDrag(dragStrategy.category);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // disable all corresponding drop zones
        //Debug.Log($"End Drag of {StaticUtilities.ListGameObjectStack(gameObject)}");
        dragAndDropState.EndDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
