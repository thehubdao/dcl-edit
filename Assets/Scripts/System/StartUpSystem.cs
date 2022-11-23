using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        [SerializeField]
        private CameraSystem _cameraSystem;

        [SerializeField]
        private SetupSceneEventListenersSystem _setupSceneEventListenersSystem;

        // Dependencies
        private AssetManagerSystem _assetManagerSystem;
        private SetupSceneSystem _setupSceneSystem;
        private WorkspaceSaveSystem _workspaceSaveSystem;
        private UnityState _unityState;
        private IPathState _pathState;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            SetupSceneSystem setupSceneSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            UnityState unityState,
            IPathState pathState)
        {
            _assetManagerSystem = assetManagerSystem;
            _setupSceneSystem = setupSceneSystem;
            _workspaceSaveSystem = workspaceSaveSystem;
            _unityState = unityState;
            _pathState = pathState;
        }

        void Awake()
        {
            _assetManagerSystem.CacheAllAssetMetadata();

            // Load default scene
            var v2Path = _pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            _setupSceneSystem.SetupScene(v2Path);
        }

        void Start()
        {
            _workspaceSaveSystem.Load(_unityState.dynamicPanelsCanvas);
        }
    }
}