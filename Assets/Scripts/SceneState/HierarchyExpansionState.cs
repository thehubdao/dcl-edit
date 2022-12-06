using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.SceneState
{
    public class HierarchyExpansionState
    {
        private Dictionary<Guid, bool> _expandedEntities = new Dictionary<Guid, bool>();
        
        private Dictionary<Guid, bool> prevExpandedState;
        private Guid prevSelectedEntity = Guid.Empty;
        
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

        public void ToggleExpanded(Guid id, bool fromUserInteraction = true)
        {
            if (_expandedEntities.TryGetValue(id, out var currentValue))
            {
                _expandedEntities[id] = !currentValue;
                if (fromUserInteraction && prevExpandedState != null)
                    prevExpandedState[id] = !currentValue;
                return;
            }

            // else
            _expandedEntities.Add(id, !_defaultExpansionState);
            if(fromUserInteraction && prevExpandedState != null)
                prevExpandedState.Add(id, !_defaultExpansionState);
        }

        public void ExpandParents(Guid selectedEntity, IEnumerable<Guid> parentList)
        {
            // Same entity selected
            if (selectedEntity == prevSelectedEntity) return;
            
            // Deselected entity - Reset
            if (selectedEntity == Guid.Empty)
            {
                _expandedEntities = prevExpandedState;
                prevSelectedEntity = Guid.Empty;
                prevExpandedState = null;
                return;
            }
            
            // New entity selected
            if (prevExpandedState != null)
                _expandedEntities = prevExpandedState;
            
            prevSelectedEntity = selectedEntity;
            prevExpandedState = new Dictionary<Guid, bool>(_expandedEntities);

            foreach (var parent in parentList.Where(parent => !IsExpanded(parent)))
            {
                ToggleExpanded(parent, false);
            }
        }
    }
}
