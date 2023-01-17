using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Assets.Scripts.SceneState
{
    public class SelectionState
    {
        [CanBeNull]
        public DclEntity PrimarySelectedEntity { get; set; }

        public List<DclEntity> SecondarySelectedEntities { get; set; } = new List<DclEntity>();

        public IEnumerable<DclEntity> AllSelectedEntities =>
            SecondarySelectedEntities
                .Prepend(PrimarySelectedEntity);
    }
}
