using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GizmoModeInteraction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Dependencies
        private GizmoState _gizmoState;
        private EditorEvents _editorEvents;

        public GameObject _translateButton;
        public GameObject _rotateButton;
        public GameObject _scaleButton;

        public bool IsMouseOverGizmoModeMenu { get; set; }

        [Inject]
        private void Construct(GizmoState gizmoState, EditorEvents editorEvents)
        {
            _gizmoState = gizmoState;
            _editorEvents = editorEvents;
            SetupListeners();
        }

        void Start()
        {
            UpdateVisuals();
        }

        public void SetupListeners()
        {
            _editorEvents.onUpdateGizmoModeMenu += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            _translateButton.GetComponent<Button>().interactable = _gizmoState.CurrentMode != GizmoState.Mode.Translate;
            _rotateButton.GetComponent<Button>().interactable = _gizmoState.CurrentMode != GizmoState.Mode.Rotate;
            _scaleButton.GetComponent<Button>().interactable = _gizmoState.CurrentMode != GizmoState.Mode.Scale;
        }

        public void SetManipulatorTranslate()
        {
            _gizmoState.CurrentMode = GizmoState.Mode.Translate;
        }
        public void SetManipulatorRotate()
        {
            _gizmoState.CurrentMode = GizmoState.Mode.Rotate;
        }
        public void SetManipulatorScale()
        {
            _gizmoState.CurrentMode = GizmoState.Mode.Scale;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            this.gameObject.GetComponent<Button>().onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseOverGizmoModeMenu = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOverGizmoModeMenu = false;
        }
    }
}
