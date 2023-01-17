using System;
using UnityEngine;
using Zenject;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Visuals
{
    public class EntityVisuals : MonoBehaviour
    {
        public Guid id;

        // Dependencies
        private GltfShapeVisuals.Factory gltfShapeVisualsFactory;
        private PrimitiveShapeVisuals.Factory primitiveShapeVisualsFactory;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            GltfShapeVisuals.Factory gltfShapeVisualsFactory,
            PrimitiveShapeVisuals.Factory primitiveShapeVisualsFactory,
            SceneManagerSystem sceneManagerSystem)
        {
            this.gltfShapeVisualsFactory = gltfShapeVisualsFactory;
            this.primitiveShapeVisualsFactory = primitiveShapeVisualsFactory;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        public void Initialize(DclScene scene)
        {
            var entity = scene?.GetEntityById(id) ?? scene?.GetFloatingEntityById(id);

            if (entity == null)
                return;

            InitializeTransformComponent(entity);
            InitializeGltfShapeVisualsComponent(entity);
            InitializePrimitiveShapeComponent(entity);
        }

        void InitializeTransformComponent(DclEntity entity)
        {
            var transformComponent = entity.GetTransformComponent();
            if (transformComponent == null)
            {
                return;
            }

                transform.localPosition = transformComponent.Position.Value;
                transform.localRotation = transformComponent.Rotation.Value;
                transform.localScale = transformComponent.Scale.Value;
            }

        void InitializeGltfShapeVisualsComponent(DclEntity entity)
        {
            var gltfShapeComponent = entity.GetComponentByName("GLTFShape");
            var gltfShapeVisualization = GetComponent<GltfShapeVisuals>(); // returns null if component isn't found

            if (gltfShapeComponent != null)
            {
                if (gltfShapeVisualization == null)
                {
                    gltfShapeVisualization = gltfShapeVisualsFactory.Create();
                }

                gltfShapeVisualization.UpdateVisuals(entity);
            }
            else if (gltfShapeVisualization != null)
            {
                gltfShapeVisualization.Deactivate();
            }
        }

        void InitializePrimitiveShapeComponent(DclEntity entity)
        {
            var primitiveShapeComponent =
                entity.GetFirstComponentByName("BoxShape", "SphereShape", "CylinderShape", "PlaneShape", "ConeShape");
            var primitiveShapeVisualization = GetComponent<PrimitiveShapeVisuals>(); // returns null if component isn't found
            if (primitiveShapeComponent != null)
            {
                if (primitiveShapeVisualization == null)
                    primitiveShapeVisualization = primitiveShapeVisualsFactory.Create();

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
