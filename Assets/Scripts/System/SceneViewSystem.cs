using Assets.Scripts.Events;
using Zenject;

namespace Assets.Scripts.System
{
    public class SceneViewSystem
    {
        // Dependencies
        private CameraSystem cameraSystem;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(
            CameraSystem cameraSystem,
            EditorEvents editorEvents)
        {
            this.cameraSystem = cameraSystem;
            this.editorEvents = editorEvents;
        }


        public void SetUpCurrentScene()
        {
            cameraSystem.CameraStartup();

            editorEvents.InvokeHierarchyChangedEvent();
        }

        public void UpdateSceneTabTitle()
        {
            editorEvents.InvokeCurrentSceneChangedEvent();
        }
    }
}
