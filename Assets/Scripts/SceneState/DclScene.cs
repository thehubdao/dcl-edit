using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.SceneState
{
    public class DclScene
    {
        public string name = "New Scene";

        private Dictionary<Guid, DclEntity> _allEntities = new Dictionary<Guid, DclEntity>();

        public IEnumerable<DclEntity> EntitiesInSceneRoot =>
            AllEntities
                .Where(e => e.Value.Parent == null)
                .Select(e => e.Value);

        public DclEntity GetEntityById(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return _allEntities.TryGetValue(id, out var entity) ? entity : null;
        }

        public IEnumerable<KeyValuePair<Guid, DclEntity>> AllEntities => _allEntities;

        public void AddEntity(DclEntity entity)
        {
            entity.Scene = this;
            _allEntities.Add(entity.Id, entity);
        }

        public void RemoveEntity(Guid id)
        {
            _allEntities.Remove(id);
        }

        // Other States

        public SelectionState SelectionState = new SelectionState();

        public CommandHistoryState CommandHistoryState = new CommandHistoryState();

        public DclComponent.DclComponentProperty GetPropertyFromIdentifier(DclPropertyIdentifier identifier)
        {
            return GetEntityById(identifier.Entity)
                .GetComponentByName(identifier.Component)
                .GetPropertyByName(identifier.Property);
        }
    }
}
