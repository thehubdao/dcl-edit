using Assets.Scripts.Events;
using Assets.Scripts.Interaction;
using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class MainSceneVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        // Dependencies
        private EntitySelectInteraction.Factory _entitySelectInteractionFactory;
        private EditorState.SceneFile _sceneFile;
        private EditorEvents _editorEvents;

        [Inject]
        public void Construct(
            EntitySelectInteraction.Factory entitySelectionInteractionFactory,
            EditorState.SceneFile sceneFile,
            EditorEvents editorEvents)
        {
            _entitySelectInteractionFactory = entitySelectionInteractionFactory;
            _sceneFile = sceneFile;
            _editorEvents = editorEvents;
        }

        public void SetupSceneEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            _editorEvents.onHierarchyChangedEvent += UpdateVisuals;

            _editorEvents.onSelectionChangedEvent += UpdateVisuals;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var scene = _sceneFile.CurrentScene;
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
                var newEntityInteraction = _entitySelectInteractionFactory.Create();
                newEntityInteraction.Id = entity.Id;

                var newEntityVisuals = newEntityInteraction.GetComponent<EntityVisuals>();
                newEntityVisuals.Id = entity.Id;

                newEntityInteraction.transform.parent = transform;

                visuals.Add(newEntityVisuals);
            }

            // set entity visual's parents
            foreach (var visual in visuals)
            {
                var parent = scene.GetEntityFormId(visual.Id).Parent; // look, if the actual entity of the visual has a parent

                if (parent != null)
                    // set the transforms parent to the transform of the parent visual
                    visual.transform.SetParent(visuals.Find(v => v.Id == parent.Id).transform, true);
            }

            // update entity visuals
            foreach (var visual in visuals)
            {
                visual.UpdateVisuals();
            }
        }
    }
}
