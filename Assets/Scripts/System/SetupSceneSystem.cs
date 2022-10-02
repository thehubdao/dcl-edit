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
        private CameraSystem _cameraSystem;
        private SetupSceneEventListenersSystem _setupSceneEventListenersSystem;
        private EditorState.SceneFile _sceneFile;
        private ISceneLoadSystem _sceneLoadSystem;

        [Inject]
        private void Construct(
            CameraSystem cameraSystem,
            SetupSceneEventListenersSystem setupSceneEventListenersSystem,
            EditorState.SceneFile sceneFile,
            ISceneLoadSystem sceneLoadSystem)
        {
            _cameraSystem = cameraSystem;
            _setupSceneEventListenersSystem = setupSceneEventListenersSystem;
            _sceneFile = sceneFile;
            _sceneLoadSystem = sceneLoadSystem;
        }

        public void SetupScene(string path)
        {
            _sceneFile.DirectoryPath = path;

            _sceneLoadSystem.Load(_sceneFile);

            _cameraSystem.CameraStartup();

            _setupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
