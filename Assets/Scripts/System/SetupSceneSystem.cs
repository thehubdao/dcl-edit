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

        [Inject]
        private void Construct(CameraSystem cameraSystem, SetupSceneEventListenersSystem setupSceneEventListenersSystem, EditorState.SceneFile sceneFile)
        {
            _cameraSystem = cameraSystem;
            _setupSceneEventListenersSystem = setupSceneEventListenersSystem;
            _sceneFile = sceneFile;
        }

        public void SetupScene(DclScene scene)
        {
            _sceneFile.CurrentScene = scene;

            _cameraSystem.CameraStartup();

            _setupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
