using System;
using UnityEngine;
using Zenject;
using Assets.Scripts.SceneState;
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

        [Inject]
        private void Construct(
            GltfShapeVisuals.Factory gltfShapeVisualsFactory,
            PrimitiveShapeVisuals.Factory primitiveShapeVisualsFactory,
            MainSceneVisuals.Factory mainSceneVisualsFactory)
        {
            this.gltfShapeVisualsFactory = gltfShapeVisualsFactory;
            this.primitiveShapeVisualsFactory = primitiveShapeVisualsFactory;
            this.mainSceneVisualsFactory = mainSceneVisualsFactory;
        }

        public void Initialize(DclScene scene)
        {
            var entity = scene?.GetEntityById(id) ?? scene?.GetFloatingEntityById(id);

            if (entity == null)
                return;

            InitializeTransformComponent(entity);
            InitializeGltfShapeVisualsComponent(entity);
            InitializePrimitiveShapeComponent(entity);
            InitializeMainSceneVisualsComponent(entity);
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

        void InitializeMainSceneVisualsComponent(DclEntity entity)
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
                        // Return instantly because the empty string is the default id value. No error should be
                        // thrown when a new component is added
                        if (dclSceneComponent.sceneId.FixedValue == "")
                        {
                            return;
                        }

                        sceneId = Guid.Parse(dclSceneComponent.sceneId.FixedValue);

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

                    mainSceneVisuals.ShowScene(sceneId);

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
