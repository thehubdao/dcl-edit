using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        // Dependencies
        private AssetManagerSystem assetManagerSystem;
        private WorkspaceSaveSystem workspaceSaveSystem;
        private UnityState unityState;
        private FrameTimeSystem frameTimeSystem;
        private SceneManagerSystem sceneManagerSystem;
        private SceneViewSystem sceneViewSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            UnityState unityState,
            IPathState pathState,
            FrameTimeSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            SceneViewSystem sceneViewSystem)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.unityState = unityState;
            this.frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneViewSystem = sceneViewSystem;
        }

        void Awake()
        {
            assetManagerSystem.CacheAllAssetMetadata();

            sceneManagerSystem.DiscoverScenes();

            // TODO: load proper scene. Work around is to load the first scene
            sceneManagerSystem.SetFirstSceneAsCurrent();

            sceneViewSystem.SetUpCurrentScene();
        }

        void Start()
        {
            workspaceSaveSystem.Load(unityState.dynamicPanelsCanvas);

            frameTimeSystem.SetApplicationTargetFramerate();
        }
    }
}