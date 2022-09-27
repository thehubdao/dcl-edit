using System;

namespace Assets.Scripts.System
{
    public class EditorEvents
    {
        public event Action onSelectionChangedEvent;
        public void SelectionChangedEvent() => onSelectionChangedEvent?.Invoke();


        public event Action onCameraStateChangedEvent;
        public void CameraStateChangedEvent() => onCameraStateChangedEvent?.Invoke();


        public event Action onHoverChangedEvent;
        public void HoverChangedEvent() => onHoverChangedEvent?.Invoke();


        public event Action onHierarchyChangedEvent;
        public void HierarchyChangedEvent() => onHierarchyChangedEvent?.Invoke();
    }
}