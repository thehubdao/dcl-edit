using Assets.Scripts.System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class TemporaryEntityVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        // Dependencies
        private EditorState.SceneState _sceneState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(EditorState.SceneState sceneState, EditorEvents editorEvents)
        {
            _sceneState = sceneState;
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