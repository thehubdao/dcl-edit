using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.SceneState
{
    public class SelectionState
    {
        public DclEntity PrimarySelectedEntity { get; set; }

        public List<DclEntity> SecondarySelectedEntities { get; set; } = new List<DclEntity>();

        public IEnumerable<DclEntity> AllSelectedEntities =>
            SecondarySelectedEntities
                .Prepend(PrimarySelectedEntity);
    }
}
