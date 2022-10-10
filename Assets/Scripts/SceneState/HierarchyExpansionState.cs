using System;
using System.Collections.Generic;

namespace Assets.Scripts.SceneState
{
    public class HierarchyExpansionState
    {
        private readonly Dictionary<Guid, bool> _expandedEntities = new Dictionary<Guid, bool>();

        private const bool _defaultExpansionState = false;

        public bool IsExpanded(Guid id)
        {
            // Returns contents of dictionary or false if key does not exist
            // This also means all entities are not expanded by default
            if (_expandedEntities.TryGetValue(id, out var isExpanded))
            {
                return isExpanded;
            }

            // else
            return _defaultExpansionState;
        }

        public void SetExpanded(Guid id, bool isExpanded)
        {
            if (_expandedEntities.TryGetValue(id, out var currentValue))
            {
                if (currentValue == isExpanded)
                {
                    return;
                }

                _expandedEntities[id] = isExpanded;
                return;
            }

            // else
            _expandedEntities.Add(id, isExpanded);
        }

        public void ToggleExpanded(Guid id)
        {
            if (_expandedEntities.TryGetValue(id, out var currentValue))
            {
                _expandedEntities[id] = !currentValue;
                return;
            }

            // else
            _expandedEntities.Add(id, !_defaultExpansionState);
        }
    }
}
