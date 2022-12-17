using Assets.Scripts.Events;
using Assets.Scripts.Interaction;
using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class MainSceneVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        // Dependencies
        private EntitySelectInteraction.Factory entitySelectInteractionFactory;
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        public void Construct(
            EntitySelectInteraction.Factory entitySelectInteractionFactory,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            this.entitySelectInteractionFactory = entitySelectInteractionFactory;
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        public void SetupSceneEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            editorEvents.onHierarchyChangedEvent += UpdateVisuals;

            editorEvents.onSelectionChangedEvent += UpdateVisuals;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            if (scene == null)
                return;

            // TODO: be smarter about caching and stuff
            foreach (var child in transform.GetChildren())
            {
                Destroy(child.gameObject);
            }

            List<EntityVisuals> visuals = new List<EntityVisuals>();

            // Generate entity visuals
            foreach (var entity in scene.AllEntities.Select(e => e.Value))
            {
                //var newEntityVisualsGameObject = Instantiate(_entityVisualsPrefab, transform);
                var newEntityInteraction = entitySelectInteractionFactory.Create();
                newEntityInteraction.id = entity.Id;

                var newEntityVisuals = newEntityInteraction.GetComponent<EntityVisuals>();
                newEntityVisuals.id = entity.Id;

                newEntityInteraction.transform.parent = transform;

                visuals.Add(newEntityVisuals);
            }

            // set entity visual's parents
            foreach (var visual in visuals)
            {
                var parent = scene.GetEntityById(visual.id).Parent; // look, if the actual entity of the visual has a parent

                if (parent != null)
                    // set the transforms parent to the transform of the parent visual
                    visual.transform.SetParent(visuals.Find(v => v.id == parent.Id).transform, true);
            }

            // update entity visuals
            foreach (var visual in visuals)
            {
                visual.UpdateVisuals();
            }
        }
    }
}
