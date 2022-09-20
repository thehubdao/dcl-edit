using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GizmoVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        private GameObject _translateGizmoObject = null;
        private GameObject _rotateGizmoObject = null;
        private GameObject _scaleGizmoObject = null;
        private GameObject activeGizmo = null;

        // dependencies
        GizmoState _gizmoState;

        [Inject]
        private void Construct(GizmoState gizmoState)
        {
            _gizmoState = gizmoState;
        }

        public void SetupSceneEventListeners()
        {
            EditorStates.CurrentSceneState.CurrentScene?.SelectionState.SelectionChangedEvent.AddListener(UpdateVisuals);
        }

        private void UpdateVisuals()
        {
            var selectedEntity = EditorStates.CurrentSceneState?
                .CurrentScene?
                .SelectionState?
                .PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                HideGizmo();
                return;
            }

            activeGizmo?.SetActive(false);
            switch (_gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    if (_translateGizmoObject == null)
                        _translateGizmoObject = Instantiate(EditorStates.CurrentUnityState.TranslateGizmoPrefab);
                    activeGizmo = _translateGizmoObject;
                    break;
                case GizmoState.Mode.Rotate:
                    if (_rotateGizmoObject == null)
                        _rotateGizmoObject = Instantiate(EditorStates.CurrentUnityState.RotateGizmoPrefab);
                    activeGizmo = _rotateGizmoObject;
                    break;
                case GizmoState.Mode.Scale:
                    if (_scaleGizmoObject == null)
                        _scaleGizmoObject = Instantiate(EditorStates.CurrentUnityState.ScaleGizmoPrefab);
                    activeGizmo = _scaleGizmoObject;
                    break;
            }
            activeGizmo.SetActive(true);

            var selectedTransform = selectedEntity.GetTransformComponent();
            activeGizmo.transform.position = selectedTransform.GlobalPosition;
            activeGizmo.transform.rotation = selectedTransform.GlobalRotation;
        }

        private void HideGizmo()
        {
            _translateGizmoObject?.SetActive(false);
            _rotateGizmoObject?.SetActive(false);
            _scaleGizmoObject?.SetActive(false);
        }
    }
}
