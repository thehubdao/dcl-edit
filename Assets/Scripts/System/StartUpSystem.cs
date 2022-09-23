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
        private ISceneLoadSystem _sceneLoadSystem;
        private SetupSceneSystem _setupSceneSystem;
        private WorkspaceSaveSystem _workspaceSaveSystem;
        private UnityState _unityState;
        private PathState _pathState;

        [Inject]
        private void Construct(ISceneLoadSystem sceneSave, SetupSceneSystem setupSceneSystem, WorkspaceSaveSystem workspaceSaveSystem, UnityState unityState, PathState pathState)
        {
            _sceneLoadSystem = sceneSave;
            _setupSceneSystem = setupSceneSystem;
            _workspaceSaveSystem = workspaceSaveSystem;
            _unityState = unityState;
            _pathState = pathState;
        }

        void Awake()
        {
            // Load scene
            var v2Path = _pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            var scene = //Directory.Exists(v2Path) ?
                _sceneLoadSystem.Load(v2Path); // :
            //LoadFromVersion1System.Load();

            _setupSceneSystem.SetupScene(scene);
        }

        void Start()
        {
            _workspaceSaveSystem.Load(_unityState.dynamicPanelsCanvas);
        }
    }
}