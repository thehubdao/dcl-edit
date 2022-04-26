using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.State
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
            return AllEntities.TryGetValue(id, out var entity) ? entity : null;
        }
    }
}
