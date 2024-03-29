using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class PrimitiveShapeVisuals : ShapeVisuals
    {
        private GameObject _colliderObject = null;

        // Dependencies
        UnityState _unityState;

        [Inject]
        private void Construct(UnityState unityState)
        {
            _unityState = unityState;
        }

        public override void UpdateVisuals(DclScene scene, DclEntity entity)
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            var meshFilter = GetComponent<MeshFilter>();


            var primitiveShapeComponent =
                entity.GetFirstComponentByName(
                    "BoxShape",
                    "SphereShape",
                    "CylinderShape",
                    "PlaneShape",
                    "ConeShape",
                    "BoxRenderer",
                    "SphereRenderer",
                    "CylinderRenderer",
                    "PlaneRenderer");

            if (primitiveShapeComponent == null)
            {
                meshRenderer.enabled = false;
                return;
            }

            meshRenderer.enabled = true;

            meshFilter.mesh = primitiveShapeComponent.NameInCode switch
            {
                "BoxShape" => _unityState.BoxMesh,
                "SphereShape" => _unityState.SphereMesh,
                "CylinderShape" => _unityState.CylinderMesh,
                "PlaneShape" => _unityState.PlaneMesh,
                "ConeShape" => _unityState.ConeMesh,
                "BoxRenderer" => _unityState.BoxMesh,
                "SphereRenderer" => _unityState.SphereMeshEcs7,
                "CylinderRenderer" => _unityState.CylinderMeshEcs7,
                "PlaneRenderer" => _unityState.PlaneMesh,
                _ => meshFilter.mesh
            };

            // TODO: Check for material

            meshRenderer.material = _unityState.DefaultMat;

            // check if collider exists

            UpdateCollider(entity, meshFilter.mesh);
            
            UpdateSelection(entity);
        }

        private void UpdateCollider(DclEntity entity, Mesh mesh)
        {
            if (mesh == null)
            {
                if (_colliderObject != null)
                {
                    _colliderObject.SetActive(false);
                }

                return;
            }


            if (_colliderObject == null)
            {
                // add click Collider
                _colliderObject = new GameObject($"{entity.CustomName}_collider")
                {
                    transform =
                    {
                        // set parent
                        parent = transform,

                        // reset transform
                        localPosition = Vector3.zero,
                        localRotation = Quaternion.identity,
                        localScale = Vector3.one
                    },

                    // set layer
                    layer = EntityClickLayer
                };

                // add mesh collider
                var newClickCollider = _colliderObject.AddComponent<MeshCollider>();

                newClickCollider.sharedMesh = mesh;
            }
            else
            {
                _colliderObject.SetActive(true);
                _colliderObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }

        public override void Deactivate()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;

            if (_colliderObject != null)
            {
                _colliderObject.SetActive(false);
            }
        }

        public class Factory : PlaceholderFactory<PrimitiveShapeVisuals> { }
    }
}
