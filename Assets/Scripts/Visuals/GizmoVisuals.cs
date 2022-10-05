using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GizmoVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        private GameObject _translateGizmoObject = null;
        private GameObject _rotateGizmoObject = null;
        private GameObject _scaleGizmoObject = null;
        private GameObject _activeGizmo = null;

        // Dependencies
        private EditorState.SceneDirectoryState _sceneDirectoryState;
        private TranslateFactory _translateFactory;
        private RotateFactory _rotateFactory;
        private ScaleFactory _scaleFactory;
        private GizmoState _gizmoState;
        private UnityState _unityState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(
            EditorState.SceneDirectoryState sceneDirectoryState,
            TranslateFactory translateFactory,
            RotateFactory rotateFactory,
            ScaleFactory scaleFactory,
            GizmoState gizmoState,
            UnityState unityState,
            EditorEvents editorEvents)
        {
            _sceneDirectoryState = sceneDirectoryState;
            _translateFactory = translateFactory;
            _rotateFactory = rotateFactory;
            _scaleFactory = scaleFactory;
            _gizmoState = gizmoState;
            _unityState = unityState;
            _editorEvents = editorEvents;
        }

        public void SetupSceneEventListeners()
        {
            _editorEvents.onSelectionChangedEvent += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            var selectedEntity = _sceneDirectoryState?
                .CurrentScene?
                .SelectionState?
                .PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                HideGizmo();
                return;
            }

            _activeGizmo?.SetActive(false);
            switch (_gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    if (_translateGizmoObject == null)
                        _translateGizmoObject = _translateFactory.Create().gameObject;
                    _activeGizmo = _translateGizmoObject;
                    break;
                case GizmoState.Mode.Rotate:
                    if (_rotateGizmoObject == null)
                        _rotateGizmoObject = _rotateFactory.Create().gameObject;
                    _activeGizmo = _rotateGizmoObject;
                    break;
                case GizmoState.Mode.Scale:
                    if (_scaleGizmoObject == null)
                        _scaleGizmoObject = _scaleFactory.Create().gameObject;
                    _activeGizmo = _scaleGizmoObject;
                    break;
            }

            _activeGizmo.SetActive(true);

            var selectedTransform = selectedEntity.GetTransformComponent();
            _activeGizmo.transform.position = selectedTransform.GlobalPosition;
            _activeGizmo.transform.rotation = selectedTransform.GlobalRotation;
        }

        private void HideGizmo()
        {
            _translateGizmoObject?.SetActive(false);
            _rotateGizmoObject?.SetActive(false);
            _scaleGizmoObject?.SetActive(false);
        }

        public class TranslateFactory : PlaceholderFactory<GizmoSizeFixerSystem>
        {
        }

        public class RotateFactory : PlaceholderFactory<GizmoSizeFixerSystem>
        {
        }

        public class ScaleFactory : PlaceholderFactory<GizmoSizeFixerSystem>
        {
        }
    }
}