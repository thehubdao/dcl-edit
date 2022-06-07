using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Assets.Scripts.SceneState
{
    public class DclScene
    {
        public Dictionary<Guid, DclEntity> AllEntities = new Dictionary<Guid, DclEntity>();

        public IEnumerable<DclEntity> EntitiesInSceneRoot => 
            AllEntities
                .Where(e => e.Value.Parent == null)
                .Select(e => e.Value);

        public DclEntity GetEntityFormId(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return AllEntities.TryGetValue(id, out var entity) ? entity : null;
        }

        public UnityEvent HierarchyChangedEvent = new UnityEvent();

        // Other States

        public SelectionState SelectionState = new SelectionState();

        public CommandHistoryState CommandHistoryState = new CommandHistoryState();
    }
}
