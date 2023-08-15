using Assets.Scripts.Events;
using System.Diagnostics;
using Zenject;

namespace Assets.Scripts.System
{
    public interface ISceneViewSystem
    {
        void SetUpCurrentScene();
    }

    public class SceneViewSystem : ISceneViewSystem
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
    }
}
