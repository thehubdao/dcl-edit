using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;

namespace Assets.Scripts.System
{
    public class SetupSceneEventListenersSystem
    {
        // Dependencies
        private UnityState unityState;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(UnityState unityState, SceneManagerSystem sceneManagerSystem)
        {
            this.unityState = unityState;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        public void SetupSceneEventListeners()
        {
            var scene = sceneManagerSystem.GetCurrentScene();

            if (scene == null)
                return;

            var sceneListenersToSetup =
                unityState.SceneVisuals.GetComponentsInChildren<ISetupSceneEventListeners>()
                    .Concat(unityState.Ui.GetComponentsInChildren<ISetupSceneEventListeners>());

            foreach (var listenerToSetup in sceneListenersToSetup)
            {
                listenerToSetup.SetupSceneEventListeners();
            }
        }
    }
}
