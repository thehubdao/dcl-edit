using Assets.Scripts.Events;
using Zenject;

namespace Assets.Scripts.System
{
    public class SceneViewSystem
    {
        // Dependencies
        private SceneManagerSystem sceneManagerSystem;
        private CameraSystem cameraSystem;
        private SetupSceneEventListenersSystem setupSceneEventListenersSystem;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(
            SceneManagerSystem sceneManagerSystem,
            CameraSystem cameraSystem,
            SetupSceneEventListenersSystem setupSceneEventListenersSystem,
            EditorEvents editorEvents)
        {
            this.sceneManagerSystem = sceneManagerSystem;
            this.cameraSystem = cameraSystem;
            this.setupSceneEventListenersSystem = setupSceneEventListenersSystem;
            this.editorEvents = editorEvents;
        }


        public void SetUpCurrentScene()
        {
            var scene = sceneManagerSystem.GetCurrentScene();

            cameraSystem.CameraStartup();

            setupSceneEventListenersSystem.SetupSceneEventListeners();

            editorEvents.InvokeHierarchyChangedEvent();
        }
    }
}
