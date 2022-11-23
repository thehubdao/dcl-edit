using System;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Visuals
{
    public class EntityVisuals : MonoBehaviour
    {
        public Guid Id;

        // Dependencies
        private GltfShapeVisuals.Factory _gltfShapeVisualsFactory;
        private PrimitiveShapeVisuals.Factory _primitiveShapeVisualsFactory;
        private EditorState.SceneDirectoryState _sceneDirectoryState;

        [Inject]
        private void Construct(
            GltfShapeVisuals.Factory gltfShapeVisualsFactory,
            PrimitiveShapeVisuals.Factory primitiveShapeVisualsFactory,
            EditorState.SceneDirectoryState sceneDirectoryState)
        {
            _gltfShapeVisualsFactory = gltfShapeVisualsFactory;
            _primitiveShapeVisualsFactory = primitiveShapeVisualsFactory;
            _sceneDirectoryState = sceneDirectoryState;
        }

        public void UpdateVisuals()
        {
            var entity = _sceneDirectoryState.CurrentScene?.GetEntityById(Id) ?? _sceneDirectoryState.CurrentScene?.GetFloatingEntityById(Id);
            if (entity == null)
                return;

            // Transform
            var transformComponent = entity.GetTransformComponent();
            if (transformComponent != null)
            {
                transform.localPosition = transformComponent.Position.Value;
                transform.localRotation = transformComponent.Rotation.Value;
                transform.localScale = transformComponent.Scale.Value;
            }

            // GLTF Shape
            var gltfShapeComponent = entity.GetComponentByName("GLTFShape");
            var gltfShapeVisualization = GetComponent<GltfShapeVisuals>(); // returns null if component isn't found
            if (gltfShapeComponent != null)
            {
                if (gltfShapeVisualization == null)
                {
                    gltfShapeVisualization = _gltfShapeVisualsFactory.Create();
                }

                gltfShapeVisualization.UpdateVisuals(entity);
            }
            else if (gltfShapeVisualization != null)
            {
                gltfShapeVisualization.Deactivate();
            }

            // Primitive Shape
            var primitiveShapeComponent =
                entity.GetFirstComponentByName("BoxShape", "SphereShape", "CylinderShape", "PlaneShape", "ConeShape");
            var primitiveShapeVisualization = GetComponent<PrimitiveShapeVisuals>(); // returns null if component isn't found
            if (primitiveShapeComponent != null)
            {
                if (primitiveShapeVisualization == null)
                    primitiveShapeVisualization = _primitiveShapeVisualsFactory.Create();

                primitiveShapeVisualization.UpdateVisuals(entity);
            }
            else if (primitiveShapeVisualization != null)
            {
                primitiveShapeVisualization.Deactivate();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EntityVisuals))]
    [CanEditMultipleObjects]
    public class LookAtPointEditor : Editor
    {


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var entityVis = target as EntityVisuals;

            if (entityVis == null)
                return;

            //var entity = _sceneState.CurrentScene?.GetEntityById(entityVis.Id);
            //
            //if (entity == null)
            //    return;
            //
            //
            //// Print Debug info
            //GUILayout.Label("--- DEBUG INFO ---");
            //
            //CustomEditorUtils.DrawEntityToGui(entity);
        }
    }
#endif
}
