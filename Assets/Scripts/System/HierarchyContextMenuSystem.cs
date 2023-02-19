using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;

namespace Assets.Scripts.System
{
    public class HierarchyContextMenuSystem
    {
        // Dependencies
        private CommandSystem commandSystem;
        private SceneManagerSystem sceneManagerSystem;
        private EntityPresetSystem entityPresetSystem;

        [Inject]
        public void Construct(
            CommandSystem commandSystem,
            SceneManagerSystem sceneManagerSystem,
            EntityPresetSystem entityPresetSystem)
        {
            this.commandSystem = commandSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.entityPresetSystem = entityPresetSystem;
        }

        public IEnumerable<EntityPresetState.EntityPreset> GetPresets()
        {
            return entityPresetSystem.availablePresets;
        }

        public void AddEntityFromPreset(EntityPresetState.EntityPreset preset, Guid parentId = default)
        {
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
                    parentId));
        }
    }
}