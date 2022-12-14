using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class HierarchyContextMenuSystem
    {
        // Dependencies
        private CommandSystem commandSystem;
        private SceneDirectoryState sceneState;
        private EntityPresetSystem entityPresetSystem;

        [Inject]
        public void Construct(
            CommandSystem commandSystem,
            SceneDirectoryState sceneState,
            EntityPresetSystem entityPresetSystem)
        {
            this.commandSystem = commandSystem;
            this.sceneState = sceneState;
            this.entityPresetSystem = entityPresetSystem;
        }

        // Add an empty entity with a specific parent
        public void AddEmptyEntity(DclEntity parent)
        {
            var selectionState = sceneState.CurrentScene.SelectionState;
            var command = commandSystem.CommandFactory.CreateAddEntity(
                "Empty Entity",
                parent.Id,
                selectionState.PrimarySelectedEntity?.Id ?? Guid.Empty,
                selectionState.SecondarySelectedEntities.Select(e => e.Id));
            commandSystem.ExecuteCommand(command);
        }

        public IEnumerable<EntityPresetSystem.AvailablePreset> GetPresets()
        {
            return entityPresetSystem.availablePresets;
        }

        public void AddEntityFromPreset(int presetIndex)
        {
            var template = entityPresetSystem.GetTemplate(presetIndex);

            Debug.Log($"Adding entity preset \"{entityPresetSystem.GetName(presetIndex)}\" (order: {presetIndex})");

            // TODO: Duplicate entity in command
        }
    }
}