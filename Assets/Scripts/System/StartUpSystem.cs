using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        [SerializeField]
        private CameraSystem _cameraSystem;

        // dependencies
        private AssetManagerSystem assetManagerSystem;
        private WorkspaceSaveSystem workspaceSaveSystem;
        private IPathState pathState;
        private FrameTimeSystem frameTimeSystem;
        private SceneManagerSystem sceneManagerSystem;
        private SceneViewSystem sceneViewSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            IPathState pathState,
            FrameTimeSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            SceneViewSystem sceneViewSystem)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneViewSystem = sceneViewSystem;
        }

        void Awake()
        {
            assetManagerSystem.CacheAllAssetMetadata();

            sceneManagerSystem.DiscoverScenes();

            // TODO: load proper scene. Work around is to load the first scene
            sceneManagerSystem.SetFirstSceneAsCurrentScene();
        }

        void Start()
        {
            workspaceSaveSystem.Load();

            frameTimeSystem.SetApplicationTargetFramerate();
        }
    }
}