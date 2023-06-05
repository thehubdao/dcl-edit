using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class SnackbarVisuals : MonoBehaviour
    {
        int maxSnackbarCount = 5;
        float itemLifetime = 5;
        List<GameObject> activeSnackbars = new List<GameObject>();

        [Header("Icons")]
        [SerializeField]
        Sprite debugIcon;
        [SerializeField]
        Sprite infoIcon;
        [SerializeField]
        Sprite successIcon;
        [SerializeField]
        Sprite warningIcon;
        [SerializeField]
        Sprite errorIcon;

        [Header("Colors")]
        [SerializeField]
        Color debugColor;
        [SerializeField]
        Color infoColor;
        [SerializeField]
        Color successColor;
        [SerializeField]
        Color warningColor;
        [SerializeField]
        Color errorColor;

        // Dependencies
        UnityState unityState;
        EditorEvents editorEvents;
        SnackbarState snackbarState;

        [Inject]
        public void Construct(UnityState unityState, EditorEvents editorEvents, SnackbarState snackbarState)
        {
            this.unityState = unityState;
            this.editorEvents = editorEvents;
            this.snackbarState = snackbarState;

            editorEvents.OnSnackbarChangedEvent += DisplayQueuedMessages;
        }

        public void DisplayQueuedMessages()
        {
            while (snackbarState.queuedMessages.Count > 0 && activeSnackbars.Count < maxSnackbarCount)
            {
                SnackbarState.Data data = snackbarState.queuedMessages.Dequeue();
                GameObject newItem = Instantiate(unityState.SnackbarItem, transform);
                newItem.transform.SetSiblingIndex(0);
                SnackbarItemHandler handler = newItem.GetComponent<SnackbarItemHandler>();

                handler.text.text = data.message;

                switch (data.type)
                {
                    case SnackbarState.MessageType.Debug:
                        handler.background.color = debugColor;
                        handler.icon.sprite = debugIcon;
                        break;
                    case SnackbarState.MessageType.Info:
                        handler.background.color = infoColor;
                        handler.icon.sprite = infoIcon;
                        break;
                    case SnackbarState.MessageType.Success:
                        handler.background.color = successColor;
                        handler.icon.sprite = successIcon;
                        break;
                    case SnackbarState.MessageType.Warning:
                        handler.background.color = warningColor;
                        handler.icon.sprite = warningIcon;
                        break;
                    case SnackbarState.MessageType.Error:
                        handler.background.color = errorColor;
                        handler.icon.sprite = errorIcon;
                        break;
                    default:
                        break;
                }

                activeSnackbars.Add(newItem);

                handler.button.onClick.AddListener(() =>
                {
                    StartCoroutine(RemoveSnackbar(newItem, 0));
                });

                StartCoroutine(RemoveSnackbar(newItem, itemLifetime));
            }
        }

        IEnumerator RemoveSnackbar(GameObject snackbarObject, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (activeSnackbars.Contains(snackbarObject))
            {
                activeSnackbars.Remove(snackbarObject);
            }
            if (snackbarObject != null)
            {
                Destroy(snackbarObject);
            }

            DisplayQueuedMessages();
        }
    }
}