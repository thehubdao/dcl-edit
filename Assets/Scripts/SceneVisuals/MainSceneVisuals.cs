using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneInteraction;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.SceneVisuals
{
    public class MainSceneVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        private GameObject _entityVisualsPrefab;

        public void SetupSceneEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            EditorStates.CurrentSceneState.CurrentScene?
                .HierarchyChangedEvent.AddListener(UpdateVisuals);

            EditorStates.CurrentSceneState.CurrentScene?
                .SelectionState.SelectionChangedEvent.AddListener(UpdateVisuals);

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var scene = EditorStates.CurrentSceneState.CurrentScene;
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
                var newEntityVisualsGameObject = Instantiate(_entityVisualsPrefab, transform);
                var newEntityVisuals = newEntityVisualsGameObject.GetComponent<EntityVisuals>();
                newEntityVisuals.Id = entity.Id;

                var newEntityInteraction = newEntityVisualsGameObject.GetComponent<EntitySelectInteraction>();
                newEntityInteraction.Id = entity.Id;

                visuals.Add(newEntityVisuals);
            }

            // set entity visual's parents
            foreach (var visual in visuals)
            {
                var parent = scene.GetEntityFormId(visual.Id).Parent; // look, if the actual entity of the visual has a parent

                if (parent != null)
                    // set the transforms parent to the transform of the parent visual
                    visual.transform.SetParent(visuals.Find(v => v.Id == parent.Id).transform,true);
            }

            // update entity visuals
            foreach (var visual in visuals)
            {
                visual.UpdateVisuals();
            }
        }
    }
}
