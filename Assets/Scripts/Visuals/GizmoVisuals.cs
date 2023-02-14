using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GizmoVisuals : MonoBehaviour
    {
        [SerializeField]
        private GameObject translateGizmoObject = null;

        [SerializeField]
        private GameObject rotateGizmoObject = null;

        [SerializeField]
        private GameObject scaleGizmoObject = null;

        private GameObject activeGizmo = null;

        // Dependencies

        private GizmoState gizmoState;
        private UnityState unityState;
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            GizmoState gizmoState,
            UnityState unityState,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            this.gizmoState = gizmoState;
            this.unityState = unityState;
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            editorEvents.onSelectionChangedEvent += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            var selectedEntity = sceneManagerSystem
                .GetCurrentScene()?
                .SelectionState
                .PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                HideGizmo();
                return;
            }

            activeGizmo?.SetActive(false);
            switch (gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    activeGizmo = translateGizmoObject;
                    break;
                case GizmoState.Mode.Rotate:
                    activeGizmo = rotateGizmoObject;
                    break;
                case GizmoState.Mode.Scale:
                    activeGizmo = scaleGizmoObject;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

            activeGizmo.SetActive(true);

            var selectedTransform = selectedEntity.GetTransformComponent();
            activeGizmo.transform.position = selectedTransform.globalPosition;
            activeGizmo.transform.rotation = selectedTransform.globalRotation;
        }

        private void HideGizmo()
        {
            translateGizmoObject?.SetActive(false);
            rotateGizmoObject?.SetActive(false);
            scaleGizmoObject?.SetActive(false);
        }
    }
}