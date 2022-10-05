using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.SceneState
{
    public class SelectionState
    {
        public DclEntity PrimarySelectedEntity { get; set; }

        private readonly List<DclEntity> _secondarySelectedEntities = new List<DclEntity>();

        public List<DclEntity> SecondarySelectedEntities => _secondarySelectedEntities;

        public IEnumerable<DclEntity> AllSelectedEntities =>
            _secondarySelectedEntities
                .Prepend(PrimarySelectedEntity);
    }
}
