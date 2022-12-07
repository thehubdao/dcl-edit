using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class AddEntitySystem
    {
        // Dependencies
        private CommandSystem commandSystem;
        private SceneDirectoryState sceneState;

        [Inject]
        public void Construct(CommandSystem commandSystem, SceneDirectoryState sceneState)
        {
            this.commandSystem = commandSystem;
            this.sceneState = sceneState;
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
    }
}
