using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Command.Utility;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEditor;
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
            
            var scene = sceneManagerSystem.GetCurrentScene();
            
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

        public void AddModelAssetEntityAsCommand(DclEntity newEntity, AssetMetadata assetMetadata, Vector3 position)
        {
            var hierarchyOrder = hierarchyOrderSystem.GetDefaultHierarchyOrder(newEntity.ParentId);
            
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateAddModelAssetToScene(newEntity.Id,
                newEntity.CustomName, assetMetadata.assetId, position, hierarchyOrder));
        }

        public void DuplicateEntityAsCommand(DclEntity selectedEntity)
        {
            var hierarchyOrder = hierarchyOrderSystem.GetDefaultHierarchyOrder(selectedEntity.ParentId);
            
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateDuplicateEntity(selectedEntity.Id, hierarchyOrder));
        }
    }
}
