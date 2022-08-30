using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

namespace Assets.Scripts.Visuals
{
    public class GizmoVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        private GameObject _translateGizmoObject = null;
        private GameObject _rotateGizmoObject = null;
        private GameObject _scaleGizmoObject = null;


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



            // TODO: switch between the different Gizmos

            if (_translateGizmoObject == null)
                _translateGizmoObject = Instantiate(EditorStates.CurrentUnityState.TranslateGizmoPrefab);

            var activeGizmo = _translateGizmoObject;

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
