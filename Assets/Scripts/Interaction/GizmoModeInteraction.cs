using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GizmoModeInteraction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Dependencies
        private GizmoToolSystem gizmoToolSystem;
        private EditorEvents editorEvents;

        public GameObject translateButton;
        public GameObject rotateButton;
        public GameObject scaleButton;

        public bool isMouseOverGizmoModeMenu { get; set; }

        [Inject]
        private void Construct(GizmoToolSystem gizmoState, EditorEvents editorEvents)
        {
            gizmoToolSystem = gizmoState;
            this.editorEvents = editorEvents;
            SetupListeners();
        }

        void Start()
        {
            UpdateVisuals();
        }

        public void SetupListeners()
        {
            editorEvents.onUpdateGizmoModeMenu += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            translateButton.GetComponent<Button>().interactable = gizmoToolSystem.gizmoToolMode != GizmoToolSystem.ToolMode.Translate;
            rotateButton.GetComponent<Button>().interactable = gizmoToolSystem.gizmoToolMode != GizmoToolSystem.ToolMode.Rotate;
            scaleButton.GetComponent<Button>().interactable = gizmoToolSystem.gizmoToolMode != GizmoToolSystem.ToolMode.Scale;
        }

        public void SetManipulatorTranslate()
        {
            gizmoToolSystem.gizmoToolMode = GizmoToolSystem.ToolMode.Translate;
        }
        public void SetManipulatorRotate()
        {
            gizmoToolSystem.gizmoToolMode = GizmoToolSystem.ToolMode.Rotate;
        }
        public void SetManipulatorScale()
        {
            gizmoToolSystem.gizmoToolMode = GizmoToolSystem.ToolMode.Scale;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            this.gameObject.GetComponent<Button>().onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOverGizmoModeMenu = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOverGizmoModeMenu = false;
        }
    }
}
