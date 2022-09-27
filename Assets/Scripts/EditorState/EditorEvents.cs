using System;

namespace Assets.Scripts.System
{
    public class EditorEvents
    {
        public event Action onSelectionChangedEvent;
        public void SelectionChangedEvent() => onSelectionChangedEvent?.Invoke();


        public event Action onCameraStateChanged;
        public void CameraStateChangedEvent() => onCameraStateChanged?.Invoke();


        public event Action onHoverChanged;
        public void HoverChangedEvent() => onHoverChanged?.Invoke();
    }
}