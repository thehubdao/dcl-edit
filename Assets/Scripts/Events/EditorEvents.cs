using System;
using System.Collections.Generic;

namespace Assets.Scripts.Events
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


        public event Action onSettingsChangedEvent;
        public void InvokeSettingsChangedEvent() => onSettingsChangedEvent?.Invoke();


        public event Action onUpdateContextMenuEvent;
        public void InvokeUpdateContextMenuEvent() => onUpdateContextMenuEvent?.Invoke();


        public event Action<int> onMouseButtonDownEvent;
        public void InvokeOnMouseButtonDownEvent(int button) => onMouseButtonDownEvent?.Invoke(button);


        public event Action<List<Guid>> onAssetDataUpdatedEvent;
        public void InvokeAssetDataUpdatedEvent(List<Guid> assetIds) => onAssetDataUpdatedEvent?.Invoke(assetIds);


        public event Action<List<Guid>> onAssetThumbnailUpdatedEvent;
        public void InvokeThumbnailDataUpdatedEvent(List<Guid> assetIds) => onAssetThumbnailUpdatedEvent?.Invoke(assetIds);


        public event Action onAssetMetadataCacheUpdatedEvent;
        public void InvokeAssetMetadataCacheUpdatedEvent() => onAssetMetadataCacheUpdatedEvent?.Invoke();
    }
}