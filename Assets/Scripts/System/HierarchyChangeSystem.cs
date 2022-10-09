using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    public class HierarchyChangeSystem
    {
        // Dependencies
        private EntitySelectSystem _entitySelectSystem;

        [Inject]
        private void Construct(EntitySelectSystem entitySelectSystem)
        {
            _entitySelectSystem = entitySelectSystem;
        }

        public void ClickedOnEntityInHierarchy(DclEntity entity)
        {
            _entitySelectSystem.ClickedOnEntity(entity.Id);
        }
    }
}
