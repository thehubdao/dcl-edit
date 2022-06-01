using System;
using Assets.Scripts.EditorState;
using UnityEngine;

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
}
