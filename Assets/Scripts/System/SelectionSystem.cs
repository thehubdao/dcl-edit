using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Assets.Scripts.System
{
    public class SelectionSystem
    {
        // Dependencies
        private SceneManagerState _sceneManagerState;

        [Inject]
        private void Construct(SceneManagerState sceneManagerState)
        {
            _sceneManagerState = sceneManagerState;
        }

        [CanBeNull]
        public DclEntity PrimarySelectedEntity => _sceneManagerState.GetCurrentDirectoryState()?.currentScene?.SelectionState.PrimarySelectedEntity;
        public List<DclEntity> SecondarySelectedEntities => _sceneManagerState.GetCurrentDirectoryState()?.currentScene?.SelectionState.SecondarySelectedEntities;

        public IEnumerable<DclEntity> AllSelectedEntities => _sceneManagerState.GetCurrentDirectoryState()?.currentScene?.SelectionState.AllSelectedEntities;

        public IEnumerable<DclEntity> AllSelectedEntitiesWithoutChildren
        {
            get
            {
                var allSelected = AllSelectedEntities.ToList();
                var selectedWithoutChildren = new List<DclEntity>();

                // Walk though entire scene tree
                var currentTreeObject = new Stack<DclEntity>();

                foreach (var entity in _sceneManagerState.GetCurrentDirectoryState().currentScene.EntitiesInSceneRoot)
                {
                    currentTreeObject.Push(entity);
                }

                while (currentTreeObject.Count > 0)
                {
                    foreach (var child in currentTreeObject.Pop().Children)
                    {
                        if (allSelected.Contains(child))
                        {
                            selectedWithoutChildren.Add(child as DclEntity); // if current object is selected, add it to returned list
                        }
                        else
                        {
                            currentTreeObject.Push(child); // else push it to the stack to be traversed
                        }
                    }
                }

                return selectedWithoutChildren.AsEnumerable();
            }
        }

    }
}