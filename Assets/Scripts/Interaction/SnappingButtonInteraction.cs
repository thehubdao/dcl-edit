using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class SnappingButtonInteraction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Dependencies
        private GizmoState _gizmoState;
        private EditorEvents _editorEvents;

        public GameObject _snappingButton;

        public bool IsMouseOverSnapppingButtonMenu{ get; set; }

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
            _editorEvents.onUpdateRelationSwitcher += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            /**
             * Define here the interactable property from each button
             * _snappingButton.GetComponent<Button>().interactable = ""
             */
            Debug.Log("Update visuals of relation switcher menu");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.gameObject.GetComponent<Button>().onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData) 
        {
            IsMouseOverSnapppingButtonMenu = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOverSnapppingButtonMenu = false;
        }
    }
}
