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
        private MainSceneVisuals.Factory mainSceneVisualsFactory;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            GltfShapeVisuals.Factory gltfShapeVisualsFactory,
            PrimitiveShapeVisuals.Factory primitiveShapeVisualsFactory,
            MainSceneVisuals.Factory mainSceneVisualsFactory,
            SceneManagerSystem sceneManagerSystem)
        {
            this.gltfShapeVisualsFactory = gltfShapeVisualsFactory;
            this.primitiveShapeVisualsFactory = primitiveShapeVisualsFactory;
            this.mainSceneVisualsFactory = mainSceneVisualsFactory;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        public void Initialize(DclScene scene, Guid? overrideSelectionId = null)
        {
            var entity = scene?.GetEntityById(id) ?? scene?.GetFloatingEntityById(id);

            if (entity == null)
                return;

            InitializeTransformComponent(entity);
            InitializeGltfShapeVisualsComponent(scene, entity);
            InitializePrimitiveShapeComponent(scene, entity);
            InitializeMainSceneVisualsComponent(entity, overrideSelectionId ?? entity.Id);
            InitializePrimarySelectionOutlineForOverrideEntity(overrideSelectionId);
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

        void InitializeGltfShapeVisualsComponent(DclScene scene, DclEntity entity)
        {
            var gltfShapeComponent = entity.GetComponentByName("GLTFShape");
            var gltfShapeVisualization = GetComponent<GltfShapeVisuals>(); // returns null if component isn't found

            if (gltfShapeComponent != null)
            {
                if (gltfShapeVisualization == null)
                {
                    gltfShapeVisualization = gltfShapeVisualsFactory.Create();
                }

                gltfShapeVisualization.UpdateVisuals(scene, entity);
            }
            else if (gltfShapeVisualization != null)
            {
                gltfShapeVisualization.Deactivate();
            }
        }

        void InitializePrimitiveShapeComponent(DclScene scene, DclEntity entity)
        {
            var primitiveShapeComponent =
                entity.GetFirstComponentByName("BoxShape", "SphereShape", "CylinderShape", "PlaneShape", "ConeShape");
            var primitiveShapeVisualization = GetComponent<PrimitiveShapeVisuals>(); // returns null if component isn't found
            if (primitiveShapeComponent != null)
            {
                if (primitiveShapeVisualization == null)
                    primitiveShapeVisualization = primitiveShapeVisualsFactory.Create();

                primitiveShapeVisualization.UpdateVisuals(scene, entity);
            }
            else if (primitiveShapeVisualization != null)
            {
                primitiveShapeVisualization.Deactivate();
            }
        }

        void InitializeMainSceneVisualsComponent(DclEntity entity, Guid overrideSelectionId)
        {
            DclComponent component = entity.GetFirstComponentByName("Scene", "Scene");
            if (component != null)
            {
                DclSceneComponent dclSceneComponent = new DclSceneComponent(component);
                MainSceneVisuals mainSceneVisuals = GetComponent<MainSceneVisuals>();
                if (dclSceneComponent != null)
                {
                    Guid sceneId;
                    try
                    {
                        // Return instantly because the empty GUID is the default id value. No error should be
                        // thrown when a new component is added
                        if (dclSceneComponent.sceneId.FixedValue == Guid.Empty)
                        {
                            return;
                        }

                        sceneId = dclSceneComponent.sceneId.FixedValue;

                        // Check for cyclic child scenes
                        MainSceneVisuals[] parentMainSceneVisuals = GetComponentsInParent<MainSceneVisuals>();
                        foreach (MainSceneVisuals parentScene in parentMainSceneVisuals)
                        {
                            if (parentScene.sceneId == sceneId)
                            {
                                throw new ArgumentException($"Cyclic scene found, you tried to load a parent scene or the scene itself as a child scene. Scene ID: {sceneId}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                        return;
                    }

                    if (mainSceneVisuals == null)
                    {
                        mainSceneVisuals = mainSceneVisualsFactory.Create();
                        mainSceneVisuals.transform.SetParent(transform, false);
                    }

                    mainSceneVisuals.ShowScene(sceneId, overrideSelectionId);

                }
                else if (mainSceneVisuals != null)
                {
                    // TODO let MainSceneVisuals handling destroying child entities and disabling components
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }
                    mainSceneVisuals.enabled = false;
                }
            }
        }

        /// <summary>
        /// Draw outline around this entity too if the entity with the given override id is selected.
        /// </summary>
        /// <param name="overrideSelectionId"></param>
        void InitializePrimarySelectionOutlineForOverrideEntity(Guid? overrideSelectionId)
        {
            if (!overrideSelectionId.HasValue)
            {
                return;
            }

            DclScene rootScene = sceneManagerSystem.GetCurrentScene();
            if (rootScene == null)
            {
                return;
            }

            ShapeVisuals shapeVisuals = GetComponent<ShapeVisuals>();
            if (shapeVisuals == null)
            {
                return;
            }

            DclEntity overrideEntity = rootScene.GetEntityById(overrideSelectionId.Value);
            if (overrideEntity == null)
            {
                return;
            }

            if (rootScene.SelectionState.PrimarySelectedEntity == overrideEntity)
            {
                shapeVisuals.ShowPrimarySelectionOutline();
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
