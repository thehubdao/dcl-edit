using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

public class UiChangedEventTrigger : MonoBehaviour
{
    // Dependencies
    EditorEvents _editorEvents;

    RectTransform rectTransform;
    Vector2 sizeLastFrame;

    [Inject]
    private void Construct(EditorEvents editorEvents)
    {
        _editorEvents = editorEvents;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Trigger the ui changed event when the size changed
        Vector2? size = rectTransform?.rect.size;
        if (size.HasValue)
        {
            if (size.Value != sizeLastFrame)
            {
                _editorEvents.InvokeUiChangedEvent();
            }
            sizeLastFrame = size.Value;
        }
    }
}
