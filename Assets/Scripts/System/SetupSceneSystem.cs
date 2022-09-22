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

        [Inject]
        private void Construct(CameraSystem cameraSystem, SetupSceneEventListenersSystem setupSceneEventListenersSystem)
        {
            _cameraSystem = cameraSystem;
            _setupSceneEventListenersSystem = setupSceneEventListenersSystem;
        }

        public void SetupScene(DclScene scene)
        {
            EditorStates.Instance.NewSceneState(scene);

            _cameraSystem.CameraStartup();

            _setupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
