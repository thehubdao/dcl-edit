using UnityEngine;
using UnityGLTF;

namespace Assets.Scripts.EditorState
{
    public class UnityState : MonoBehaviour
    {
        [SerializeField]
        public GameObject SceneVisuals;

        [SerializeField]
        public AsyncCoroutineHelper AsyncCoroutineHelper;

        [SerializeField]
        public Mesh BoxMesh;

        [SerializeField]
        public Mesh SphereMesh;

        [SerializeField]
        public Mesh CylinderMesh;

        [SerializeField]
        public Mesh PlaneMesh;

        [SerializeField]
        public Mesh ConeMesh;

        [SerializeField]
        public Material DefaultMat;

        [Header("Gizmo Prefabs")]
        [SerializeField] 
        public GameObject TranslateGizmoPrefab;

        [SerializeField]
        public GameObject RotateGizmoPrefab;

        [SerializeField]
        public GameObject ScaleGizmoPrefab;
    }
}
