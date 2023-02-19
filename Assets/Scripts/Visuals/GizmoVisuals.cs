using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GizmoVisuals : MonoBehaviour
    {
        private GameObject translateGizmoObject = null;
        private GameObject rotateGizmoObject = null;
        private GameObject scaleGizmoObject = null;
        private GameObject activeGizmo = null;

        // Dependencies

        private TranslateFactory translateFactory;
        private RotateFactory rotateFactory;
        private ScaleFactory scaleFactory;
        private GizmoState gizmoState;
        private UnityState unityState;
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            TranslateFactory translateFactory,
            RotateFactory rotateFactory,
            ScaleFactory scaleFactory,
            GizmoState gizmoState,
            UnityState unityState,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            this.translateFactory = translateFactory;
            this.rotateFactory = rotateFactory;
            this.scaleFactory = scaleFactory;
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
                .GetCurrentSceneOrNull()?
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
                    if (translateGizmoObject == null)
                        translateGizmoObject = translateFactory.Create().gameObject;
                    activeGizmo = translateGizmoObject;
                    break;
                case GizmoState.Mode.Rotate:
                    if (rotateGizmoObject == null)
                        rotateGizmoObject = rotateFactory.Create().gameObject;
                    activeGizmo = rotateGizmoObject;
                    break;
                case GizmoState.Mode.Scale:
                    if (scaleGizmoObject == null)
                        scaleGizmoObject = scaleFactory.Create().gameObject;
                    activeGizmo = scaleGizmoObject;
                    break;
            }

            activeGizmo.SetActive(true);

            var selectedTransform = selectedEntity.GetTransformComponent();
            activeGizmo.transform.position = selectedTransform.GlobalPosition;
            activeGizmo.transform.rotation = selectedTransform.GlobalRotation;
        }

        private void HideGizmo()
        {
            translateGizmoObject?.SetActive(false);
            rotateGizmoObject?.SetActive(false);
            scaleGizmoObject?.SetActive(false);
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