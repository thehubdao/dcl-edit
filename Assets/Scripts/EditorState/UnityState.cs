using DynamicPanels;
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

        [SerializeField]
        public DynamicPanelsCanvas dynamicPanelsCanvas;

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

        [SerializeField]
        public Texture2D DefaultAssetThumbnail;

        [Space]
        [SerializeField]
        public GameObject ErrorModel;

        [Header("Ui Atoms")]
        [SerializeField]
        public GameObject ButtonAtom;

        [SerializeField]
        public GameObject AssetButton;

        [SerializeField]
        public GameObject GridAtom;

        [SerializeField]
        public GameObject RowAtom;

        [SerializeField]
        public GameObject TitleAtom;

        [SerializeField]
        public GameObject TextAtom;

        [SerializeField]
        public GameObject PanelAtom;

        [SerializeField]
        public GameObject PanelHeaderAtom;

        [SerializeField]
        public GameObject HierarchyItemAtom;

        [SerializeField]
        public GameObject StringInputAtom;

        [SerializeField]
        public GameObject NumberInputAtom;

        [SerializeField]
        public GameObject BooleanInputAtom;

        [SerializeField]
        public GameObject Vector3InputAtom;
    }
}
