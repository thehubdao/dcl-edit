using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class TemporaryEntityVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        // Dependencies
        private EditorState.SceneState _sceneState;

        [Inject]
        private void Construct(EditorState.SceneState sceneState)
        {
            _sceneState = sceneState;
        }

        public void SetupSceneEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            _sceneState.CurrentScene?
                .HierarchyChangedEvent.AddListener(UpdateVisuals);

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (_sceneState.CurrentScene == null)
                return;

            foreach (var entity in _sceneState.CurrentScene.AllEntities.Select(e => e.Value))
            {
                var entityPos = entity.GetComponentByName("transform")?.GetPropertyByName("position")
                    ?.GetConcrete<Vector3>().Value;

                if (entityPos != null)
                    Debug.DrawRay(entityPos.Value, Vector3.up, Color.red, 100);
            }
        }
    }
}