using System;
using Assets.Scripts.Events;
using Assets.Scripts.Interaction;
using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class MainSceneVisuals : MonoBehaviour
    {
        // Dependencies
        private EntitySelectInteraction.Factory entitySelectInteractionFactory;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        public void Construct(
            EntitySelectInteraction.Factory entitySelectInteractionFactory,
            SceneManagerSystem sceneManagerSystem)
        {
            this.entitySelectInteractionFactory = entitySelectInteractionFactory;
            this.sceneManagerSystem = sceneManagerSystem;
        }


        public void ShowScene(Guid sceneId)
        {
            var scene = sceneManagerSystem.GetScene(sceneId);

            // TODO: be smarter about caching and stuff
            foreach (var child in transform.GetChildren())
            {
                Destroy(child.gameObject);
            }

            List<EntityVisuals> visuals = new List<EntityVisuals>();

            // Generate entity visuals
            foreach (var entity in scene.AllEntities.Concat(scene.AllFloatingEntities).Select(e => e.Value))
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
                var parent = scene.GetEntityById(visual.id)?.Parent; // look, if the actual entity of the visual has a parent

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

        public class Factory : PlaceholderFactory<MainSceneVisuals>
        {
        }
    }
}
