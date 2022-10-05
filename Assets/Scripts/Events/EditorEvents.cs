using System;

namespace Assets.Scripts.System
{
    public class EditorEvents
    {
        public event Action onSelectionChangedEvent;
        public void InvokeSelectionChangedEvent() => onSelectionChangedEvent?.Invoke();


        public event Action onCameraStateChangedEvent;
        public void InvokeCameraStateChangedEvent() => onCameraStateChangedEvent?.Invoke();


        public event Action onHoverChangedEvent;
        public void InvokeHoverChangedEvent() => onHoverChangedEvent?.Invoke();


        public event Action onHierarchyChangedEvent;
        public void InvokeHierarchyChangedEvent() => onHierarchyChangedEvent?.Invoke();
    }
}