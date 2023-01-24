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

        [SerializeField]
        public Texture2D AssetTypeModelIcon;

        [SerializeField]
        public Texture2D AssetTypeImageIcon;

        [Space]
        [SerializeField]
        public GameObject ErrorModel;

        [SerializeField]
        public GameObject LoadingModel;


        [Header("Context Menu Prefab")]
        [SerializeField]
        public GameObject ContextMenuAtom;


        [Header("Ui Atoms")]
        [SerializeField]
        public GameObject ButtonAtom;

        [SerializeField]
        public GameObject AssetBrowserButtonAtom;

        [SerializeField]
        public GameObject AssetBrowserFolderAtom;

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
        public GameObject PanelWithBorderAtom;

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

        [SerializeField]
        public GameObject AssetInputAtom;

        [SerializeField]
        public GameObject MenuBarButtonAtom;

        [SerializeField]
        public GameObject ContextMenuItemAtom;

        [SerializeField]
        public GameObject ContextSubmenuItemAtom;

        [SerializeField]
        public GameObject ContextMenuSpacerItemAtom;


        [Header("Dialog Windows")]
        [SerializeField]
        public GameObject assetDialog;
    }
}
