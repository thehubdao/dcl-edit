using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.SceneState
{
    public class DclScene
    {
        private Dictionary<Guid, DclEntity> _allEntities = new Dictionary<Guid, DclEntity>();
        private Dictionary<Guid, DclEntity> _allFloatingEntities = new Dictionary<Guid, DclEntity>();

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
        public DclEntity GetFloatingEntityById(Guid id)
        {
            if (id == Guid.Empty) return null;

            return _allFloatingEntities.TryGetValue(id, out var entity) ? entity : null;
        }

        public bool? IsFloatingEntity(Guid id)
        {
            if (GetEntityById(id) != null) return false;
            if (GetFloatingEntityById(id) != null) return true;
            return null;
        }

        public IEnumerable<KeyValuePair<Guid, DclEntity>> AllEntities => _allEntities;
        public IEnumerable<KeyValuePair<Guid, DclEntity>> AllFloatingEntities => _allFloatingEntities;

        public void AddEntity(DclEntity entity)
        {
            entity.Scene = this;
            _allEntities.Add(entity.Id, entity);
        }
        public void AddFloatingEntity(DclEntity entity)
        {
            entity.Scene = this;
            _allFloatingEntities.Add(entity.Id, entity);
        }

        public void RemoveEntity(Guid id)
        {
            foreach (var child in _allEntities[id].Children.ToList())
            {
                RemoveEntity(child.Id);
            }
            _allEntities.Remove(id);
        }
        public void RemoveFloatingEntity(Guid id)
        {
            _allFloatingEntities.Remove(id);
        }
        public void ClearFloatingEntities() => _allFloatingEntities.Clear();

        public DclScene DeepCopy()
        {
            DclScene copy = new DclScene();
            Random random = new Random();

            foreach (KeyValuePair<Guid, DclEntity> entitiy in _allEntities)
            {
                entitiy.Value.DeepCopy(copy, random);
            }

            return copy;
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
