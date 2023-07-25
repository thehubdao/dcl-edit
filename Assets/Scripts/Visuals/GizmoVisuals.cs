using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
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
        private SceneManagerSystem sceneManagerSystem;
        private SceneManagerState sceneManagerState;

        [Inject]
        private void Construct(
            GizmoToolSystem gizmoToolSystem,
            SceneManagerSystem sceneManagerSystem,
            SceneManagerState sceneManagerState)
        {
            this.gizmoToolSystem = gizmoToolSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneManagerState = sceneManagerState;
        }

        private void OnEnable()
        {
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged += UpdateVisuals;
            sceneManagerState.currentSceneIndex.OnValueChanged += SubscribeToNewScene;
            UpdateVisuals();
        }

        private void OnDisable()
        {
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged -= UpdateVisuals;
            sceneManagerState.currentSceneIndex.OnValueChanged -= SubscribeToNewScene;
        }

        private void SubscribeToNewScene()
        {
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged -= UpdateVisuals;
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            var selectedEntity = sceneManagerSystem
                .GetCurrentSceneOrNull()?
                .SelectionState
                .PrimarySelectedEntity.Value;

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