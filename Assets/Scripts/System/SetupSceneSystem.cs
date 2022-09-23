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
        private EditorState.SceneState _sceneState;

        [Inject]
        private void Construct(CameraSystem cameraSystem, SetupSceneEventListenersSystem setupSceneEventListenersSystem, EditorState.SceneState sceneState)
        {
            _cameraSystem = cameraSystem;
            _setupSceneEventListenersSystem = setupSceneEventListenersSystem;
            _sceneState = sceneState;
        }

        public void SetupScene(DclScene scene)
        {
            _sceneState.CurrentScene = scene;

            _cameraSystem.CameraStartup();

            _setupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
