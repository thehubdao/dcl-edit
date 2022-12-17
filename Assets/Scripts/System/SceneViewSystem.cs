using Zenject;

namespace Assets.Scripts.System
{
    public class SceneViewSystem
    {
        // Dependencies
        private SceneManagerSystem sceneManagerSystem;
        private CameraSystem cameraSystem;
        private SetupSceneEventListenersSystem setupSceneEventListenersSystem;

        [Inject]
        private void Construct(
            SceneManagerSystem sceneManagerSystem,
            CameraSystem cameraSystem,
            SetupSceneEventListenersSystem setupSceneEventListenersSystem)
        {
            this.sceneManagerSystem = sceneManagerSystem;
            this.cameraSystem = cameraSystem;
            this.setupSceneEventListenersSystem = setupSceneEventListenersSystem;
        }


        public void SetUpCurrentScene()
        {
            var scene = sceneManagerSystem.GetCurrentScene();

            cameraSystem.CameraStartup();

            setupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
