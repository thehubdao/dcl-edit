using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class AddEntitySystem
    {
        // Dependencies
        private CommandSystem commandSystem;
        private SceneManagerSystem sceneManagerSystem;
        private HierarchyOrderSystem hierarchyOrderSystem;

        [Inject]
        private void Construct(CommandSystem commandSystem, SceneManagerSystem sceneManagerSystem, HierarchyOrderSystem hierarchyOrderSystem)
        {
            this.commandSystem = commandSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.hierarchyOrderSystem = hierarchyOrderSystem;
        }

        public void AddEntityFromPresetAsCommand(EntityPresetState.EntityPreset preset, Guid parentId)
        {
            var newHierarchyOrder = hierarchyOrderSystem.GetDefaultHierarchyOrder(parentId);

            var scene = sceneManagerSystem.GetCurrentSceneOrNull();

            if (scene == null)
            {
                return;
            }

            commandSystem.ExecuteCommand(
                commandSystem.CommandFactory.CreateAddEntity(
                    preset,
                    scene.SelectionState.PrimarySelectedEntity?.Id ?? Guid.Empty,
                    scene.SelectionState.SecondarySelectedEntities.Select(e => e.Id),
                    newHierarchyOrder, parentId));
        }

        public void AddModelAssetEntityAsCommand(string name, Guid assetId, Vector3 position)
        {
            var hierarchyOrder = hierarchyOrderSystem.GetDefaultHierarchyOrder(default);

            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateAddModelAssetToScene(
                Guid.NewGuid(),
                name,
                assetId,
                position,
                hierarchyOrder));
        }

        public void AddSceneAssetEntityAsCommand(string name, Guid assetId, Vector3 position)
        {
            var hierarchyOrder = hierarchyOrderSystem.GetDefaultHierarchyOrder(default);

            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateAddSceneAssetToScene(
                Guid.NewGuid(),
                name,
                assetId,
                position,
                hierarchyOrder));
        }

        public void DuplicateEntityAsCommand(DclEntity selectedEntity)
        {
            float hierarchyOrder;

            var belowEntity = hierarchyOrderSystem.GetBelowSibling(selectedEntity);

            if (belowEntity == null)
            {
                hierarchyOrder = hierarchyOrderSystem.GetHierarchyOrderPlaceBeneathSibling(selectedEntity);
            }
            else
            {
                hierarchyOrder = hierarchyOrderSystem.GetHierarchyOrderPlaceBetweenSiblings(selectedEntity, belowEntity);
            }

            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateDuplicateEntity(selectedEntity.Id, hierarchyOrder));
        }
    }
}
