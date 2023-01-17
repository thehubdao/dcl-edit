using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    public class EntityPresetSystem
    {
        public struct AvailablePreset
        {
            public int index;
            public string name;
        }

        // Dependencies
        private EntityPresetState entityPresetState;

        [Inject]
        private void Construct(EntityPresetState entityPresetState)
        {
            this.entityPresetState = entityPresetState;
        }


        public IEnumerable<EntityPresetState.EntityPreset> availablePresets => entityPresetState.allEntityPresets;

        public DclEntity GetTemplate(int index)
        {
            //return entityPresets[index].templateEntity;
            throw new NotImplementedException();
        }

        public string GetName(int index)
        {
            //return entityPresets[index].name;
            throw new NotImplementedException();
        }
    }
}
