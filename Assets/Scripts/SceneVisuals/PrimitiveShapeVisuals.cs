using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

namespace Assets.Scripts.SceneVisuals
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class PrimitiveShapeVisuals : MonoBehaviour
    {


        public void UpdateVisuals(DclEntity entity)
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            var meshFilter = GetComponent<MeshFilter>();


            var primitiveShapeComponent =
                entity.GetFirstComponentByName("BoxShape", "SphereShape", "CylinderShape", "PlaneShape", "ConeShape");

            if (primitiveShapeComponent == null)
                return;

            meshRenderer.enabled = true;

            meshFilter.mesh = primitiveShapeComponent.NameInCode switch
            {
                "BoxShape" => EditorStates.CurrentUnityState.BoxMesh,
                "SphereShape" => EditorStates.CurrentUnityState.SphereMesh,
                "CylinderShape" => EditorStates.CurrentUnityState.CylinderMesh,
                "PlaneShape" => EditorStates.CurrentUnityState.PlaneMesh,
                "ConeShape" => EditorStates.CurrentUnityState.ConeMesh,
                _ => meshFilter.mesh
            };

            // TODO: Check for material

            meshRenderer.material = EditorStates.CurrentUnityState.DefaultMat;
        }

        public void Deactivate()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;

        }
    }
}
