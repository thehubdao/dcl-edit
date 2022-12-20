using System.Security.Policy;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class SetupSceneSystem
    {
        // Dependencies
        private MenuBarSystem _menuBarSystem;
        private CameraSystem _cameraSystem;
        private SetupSceneEventListenersSystem _setupSceneEventListenersSystem;
        private EditorState.SceneDirectoryState _sceneDirectoryState;
        private ISceneLoadSystem _sceneLoadSystem;

        [Inject]
        private void Construct(
            MenuBarSystem menuBarSystem,
            CameraSystem cameraSystem,
            SetupSceneEventListenersSystem setupSceneEventListenersSystem,
            EditorState.SceneDirectoryState sceneDirectoryState,
            ISceneLoadSystem sceneLoadSystem)
        {
            _menuBarSystem = menuBarSystem;
            _cameraSystem = cameraSystem;
            _setupSceneEventListenersSystem = setupSceneEventListenersSystem;
            _sceneDirectoryState = sceneDirectoryState;
            _sceneLoadSystem = sceneLoadSystem;
        }

        public void SetupScene(string path)
        {
            _sceneDirectoryState.DirectoryPath = path;

            _sceneLoadSystem.Load(_sceneDirectoryState);

            _cameraSystem.CameraStartup();

            _setupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
