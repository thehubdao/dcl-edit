using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Zenject;

namespace Assets.Scripts.System
{
    public class HierarchyContextMenuSystem
    {
        // Dependencies
        private EntityPresetSystem entityPresetSystem;
        private AddEntitySystem addEntitySystem;

        [Inject]
        public void Construct(
            EntityPresetSystem entityPresetSystem,
            AddEntitySystem addEntitySystem)
        {
            this.entityPresetSystem = entityPresetSystem;
            this.addEntitySystem = addEntitySystem;
        }

        public IEnumerable<EntityPresetState.EntityPreset> GetPresets()
        {
            return entityPresetSystem.availablePresets;
        }

        public void AddEntityFromPreset(EntityPresetState.EntityPreset preset, Guid parentId = default)
        {
            addEntitySystem.AddEntityFromPresetAsCommand(preset, parentId);
        }
    }
}
