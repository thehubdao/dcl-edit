using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.SceneVisuals
{
    public class EntityVisuals : MonoBehaviour
    {
        public Guid Id;

        public void UpdateVisuals()
        {
            var entity = EditorStates.CurrentSceneState.CurrentScene?.GetEntityFormId(Id);
            if (entity == null)
                return;

            // Transform
            var transformComponent = entity.GetComponentByName("transform");
            if (transformComponent != null)
            {
                transform.position = transformComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value;
                transform.rotation = transformComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value;
                transform.localScale = transformComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value;
            }

            // GLTF Shape
            var gltfShapeComponent = entity.GetComponentByName("GLTFShape");
            var gltfShapeVisualization = GetComponent<GltfShapeVisuals>(); // returns null if component isn't found
            if (gltfShapeComponent != null)
            {
                if (gltfShapeVisualization == null)
                    gltfShapeVisualization = gameObject.AddComponent<GltfShapeVisuals>();

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
                    primitiveShapeVisualization = gameObject.AddComponent<PrimitiveShapeVisuals>();

                primitiveShapeVisualization.UpdateVisuals(entity);
            }
            else if (primitiveShapeVisualization != null)
            {
                primitiveShapeVisualization.Deactivate();
            }
        }
    }

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

            var entity = EditorStates.CurrentSceneState.CurrentScene?.GetEntityFormId(entityVis.Id);

            if (entity == null)
                return;


            // Print Debug info
            GUILayout.Label("--- DEBUG INFO ---");

            CustomEditorUtils.DrawEntityToGui(entity);
        }
    }
}
