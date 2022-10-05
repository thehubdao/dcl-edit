using Assets.Scripts.Events;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class TemporaryEntityVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        // Dependencies
        private EditorState.SceneFile _sceneFile;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(EditorState.SceneFile sceneFile, EditorEvents editorEvents)
        {
            _sceneFile = sceneFile;
            _editorEvents = editorEvents;
        }

        public void SetupSceneEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            _editorEvents.onHierarchyChangedEvent += UpdateVisuals;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (_sceneFile.CurrentScene == null)
                return;

            foreach (var entity in _sceneFile.CurrentScene.AllEntities.Select(e => e.Value))
            {
                var entityPos = entity.GetComponentByName("transform")?.GetPropertyByName("position")
                    ?.GetConcrete<Vector3>().Value;

                if (entityPos != null)
                    Debug.DrawRay(entityPos.Value, Vector3.up, Color.red, 100);
            }
        }
    }
}