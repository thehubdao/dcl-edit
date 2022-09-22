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

        [Inject]
        private void Construct(CameraSystem cameraSystem)
        {
            _cameraSystem = cameraSystem;
        }

        public void SetupScene(DclScene scene)
        {
            EditorStates.Instance.NewSceneState(scene);

            _cameraSystem.CameraStartup();

            SetupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
