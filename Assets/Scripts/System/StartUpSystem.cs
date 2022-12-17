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

        // dependencies
        private WorkspaceSaveSystem _workspaceSaveSystem;
        private UnityState _unityState;
        private IPathState _pathState;
        private FrameTimeSystem _frameTimeSystem;
        private SceneManagerSystem sceneManagerSystem;
        private SceneViewSystem sceneViewSystem;

        [Inject]
        private void Construct(
            WorkspaceSaveSystem workspaceSaveSystem,
            UnityState unityState,
            IPathState pathState,
            FrameTimeSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            SceneViewSystem sceneViewSystem)
        {
            _workspaceSaveSystem = workspaceSaveSystem;
            _unityState = unityState;
            _pathState = pathState;
            _frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneViewSystem = sceneViewSystem;
        }

        void Awake()
        {
            sceneManagerSystem.DiscoverScenes();

            // TODO: load proper scene. Work around is to load the first scene
            sceneManagerSystem.SetFirstSceneAsCurrent();

            sceneViewSystem.SetUpCurrentScene();
        }

        void Start()
        {
            _workspaceSaveSystem.Load(_unityState.dynamicPanelsCanvas);

            _frameTimeSystem.SetApplicationTargetFramerate();
        }
    }
}