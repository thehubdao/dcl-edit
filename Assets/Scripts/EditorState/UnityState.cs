using UnityEngine;
using UnityEngine.UI;
using UnityGLTF;

namespace Assets.Scripts.EditorState
{
    public class UnityState : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        public GameObject SceneVisuals;

        [SerializeField]
        public GameObject Ui;

        [SerializeField]
        public Camera MainCamera;

        [SerializeField]
        public RawImage SceneImage;

        [SerializeField]
        public AsyncCoroutineHelper AsyncCoroutineHelper;

        [Header("Assets")]
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

        [Space]
        [SerializeField]
        public Material DefaultMat;

        [Header("Gizmo Prefabs")]
        [SerializeField]
        public GameObject TranslateGizmoPrefab;

        [SerializeField]
        public GameObject RotateGizmoPrefab;

        [SerializeField]
        public GameObject ScaleGizmoPrefab;

        [Header("Ui Atoms")] 
        [SerializeField]
        public GameObject TitleAtom;

    }
}
