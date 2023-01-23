using Assets.Scripts.Interaction;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class MainSceneVisuals : MonoBehaviour
    {
        public Guid sceneId { get; private set; }

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

        /// <summary>
        /// Generates the scenes entity visuals. When an overrideSelectionId is given, clicking any of the scenes entities results in selecting the scene object.
        /// </summary>
        /// <param name="sceneId">Id of the scene that is generated.</param>
        /// <param name="overrideSelectionId">Id of the entity that is selected when any of the scenes entities is clicked. This id must be of an entity in the root scene.</param>
        public void ShowScene(Guid sceneId, Guid? overrideSelectionId = null)
        {
            var scene = sceneManagerSystem.GetScene(sceneId);
            if (scene == null)
            {
                return;
            }
            this.sceneId = sceneId;

            RemoveChildGameObjects();
            List<EntityVisuals> visuals = GenerateEntityVisuals(scene, overrideSelectionId);
            SetEntityVisualsParents(scene, visuals);
            InitializeEntityVisuals(scene, visuals, overrideSelectionId);
        }

        private void RemoveChildGameObjects()
        {
            foreach (var interaction in GetComponentsInChildren<EntitySelectInteraction>())
            {
                interaction.DestroyToPool();
            }
        }

        private List<EntityVisuals> GenerateEntityVisuals(DclScene scene, Guid? overrideSelectionId)
        {
            List<EntityVisuals> entityVisuals = new List<EntityVisuals>();
            foreach (var entity in scene.AllEntities.Concat(scene.AllFloatingEntities).Select(e => e.Value))
            {
                var newEntityInteraction = entitySelectInteractionFactory.Create();
                newEntityInteraction.id = overrideSelectionId ?? entity.Id;
                var newEntityVisuals = newEntityInteraction.GetComponent<EntityVisuals>();
                newEntityVisuals.id = entity.Id;

                newEntityInteraction.transform.parent = transform;

                entityVisuals.Add(newEntityVisuals);
            }
            return entityVisuals;
        }

        private void SetEntityVisualsParents(DclScene scene, List<EntityVisuals> entityVisuals)
        {
            foreach (var visual in entityVisuals)
            {
                var parent = scene.GetEntityById(visual.id)?.Parent; // look, if the actual entity of the visual has a parent

                if (parent != null)
                    // set the transforms parent to the transform of the parent visual
                    visual.transform.SetParent(entityVisuals.Find(v => v.id == parent.Id).transform, true);
            }
        }

        private void InitializeEntityVisuals(DclScene scene, List<EntityVisuals> entityVisuals, Guid? overrideSelectionId)
        {
            foreach (var visual in entityVisuals)
            {
                visual.Initialize(scene, overrideSelectionId);
            }
        }


        public class Factory : PlaceholderFactory<MainSceneVisuals>
        {
        }
    }
}
