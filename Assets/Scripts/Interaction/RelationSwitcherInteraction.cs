using System;
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Zenject;
using UnityEngine.UI;

namespace Assets.Scripts.Interaction
{
    public class RelationSwitcherInteraction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Dependencies
        private GizmoState _gizmoState;
        private EditorEvents _editorEvents;

        public GameObject _localButton;
        public GameObject _globalButton;

        public bool IsMouseOverRelationSwitcherMenu{ get; set; }


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
             * _localButton.GetComponent<Button>().interactable = "";
             * _globalButton.GetComponent<Button>().interactable = "";
             */
            Debug.Log("Update visuals of relation switcher menu");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.gameObject.GetComponent<Button>().onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseOverRelationSwitcherMenu = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOverRelationSwitcherMenu = false;
        }
    }
}
