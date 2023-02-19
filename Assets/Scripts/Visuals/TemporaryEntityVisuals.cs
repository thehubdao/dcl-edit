using Assets.Scripts.Events;
using System.Linq;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class TemporaryEntityVisuals : MonoBehaviour
    {
        // Dependencies
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            editorEvents.onHierarchyChangedEvent += UpdateVisuals;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var scene = sceneManagerSystem.GetCurrentSceneOrNull();

            if (scene == null)
            {
                return;
            }

            foreach (var entity in scene.AllEntities.Select(e => e.Value))
            {
                var entityPos = entity.GetComponentByName("transform")?.GetPropertyByName("position")
                    ?.GetConcrete<Vector3>().Value;

                if (entityPos != null)
                    Debug.DrawRay(entityPos.Value, Vector3.up, Color.red, 100);
            }
        }
    }
}