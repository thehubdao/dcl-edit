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

        private GizmoToolSystem gizmoToolSystem;
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            GizmoToolSystem gizmoToolSystem,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            this.gizmoToolSystem = gizmoToolSystem;
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
                .GetCurrentSceneOrNull()?
                .SelectionState
                .PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                HideGizmo();
                return;
            }

            activeGizmo?.SetActive(false);
            switch (gizmoToolSystem.gizmoToolMode)
            {
                case GizmoToolSystem.ToolMode.Translate:
                    activeGizmo = translateGizmoObject;
                    break;
                case GizmoToolSystem.ToolMode.Rotate:
                    activeGizmo = rotateGizmoObject;
                    break;
                case GizmoToolSystem.ToolMode.Scale:
                    activeGizmo = scaleGizmoObject;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

            activeGizmo.SetActive(true);

            var selectedTransform = selectedEntity.GetTransformComponent();
            activeGizmo.transform.position = selectedTransform.globalPosition;

            var isToolContextLocal =
                gizmoToolSystem.gizmoToolContext == GizmoToolSystem.ToolContext.Local ||
                gizmoToolSystem.gizmoToolMode == GizmoToolSystem.ToolMode.Scale;

            activeGizmo.transform.rotation =
                isToolContextLocal ?
                    selectedTransform.globalRotation :
                    Quaternion.identity;
        }

        private void HideGizmo()
        {
            translateGizmoObject?.SetActive(false);
            rotateGizmoObject?.SetActive(false);
            scaleGizmoObject?.SetActive(false);
        }
    }
}