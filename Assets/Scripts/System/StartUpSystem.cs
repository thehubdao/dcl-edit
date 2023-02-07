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
        private SettingsSystem settingsSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            IPathState pathState,
            FrameTimeSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            SceneViewSystem sceneViewSystem,
            SettingsSystem settingsSystem)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneViewSystem = sceneViewSystem;
            this.settingsSystem = settingsSystem;
        }

        void Awake()
        {
            assetManagerSystem.CacheAllAssetMetadata();

            sceneManagerSystem.DiscoverScenes();

            // TODO: load proper scene. Work around is to load the first scene
            //sceneManagerSystem.SetFirstSceneAsCurrentScene();

            sceneManagerSystem.SetLastOpenedSceneAsCurrentScene(settingsSystem.openLastOpenedScene);
        }

        void Start()
        {
            workspaceSaveSystem.Load();

            frameTimeSystem.SetApplicationTargetFramerate();
        }
    }
}