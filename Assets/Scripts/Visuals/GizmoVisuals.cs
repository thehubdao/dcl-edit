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


            // Move the Active Gizmo to the correct global position
            activeGizmo.transform.position = Vector3.zero;
            activeGizmo.transform.rotation = Quaternion.identity;

            var parentList = new Stack<DclEntity>();
            parentList.Push(selectedEntity);

            while (parentList.Peek().Parent != null)
            {
                parentList.Push(parentList.Peek().Parent);
            }

            while (parentList.Count > 0)
            {
                var e = parentList.Pop();
                var t = e.GetTransformComponent();
                if (t == null)
                    continue;
                
                activeGizmo.transform.Translate(t.Position.Value);
                activeGizmo.transform.localRotation *= t.Rotation.Value;
            }
        }

        private void HideGizmo()
        {
            _translateGizmoObject?.SetActive(false);
            _rotateGizmoObject?.SetActive(false);
            _scaleGizmoObject?.SetActive(false);
        }
    }
}
