using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class HierarchyNavigationInteraction
    {
        //Dependencies
        private SceneManagerSystem sceneManagerSystem;
        private HierarchyOrderSystem hierarchyOrderSystem;
        private EntitySelectSystem entitySelectSystem;
        private HierarchyExpansionState hierarchyExpansionState;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(
            SceneManagerSystem sceneManagerSystem,
            HierarchyOrderSystem hierarchyOrderSystem, EntitySelectSystem entitySelectSystem,
            HierarchyExpansionState hierarchyExpansionState, EditorEvents editorEvents)
        {
            this.sceneManagerSystem = sceneManagerSystem;
            this.hierarchyOrderSystem = hierarchyOrderSystem;
            this.entitySelectSystem = entitySelectSystem;
            this.hierarchyExpansionState = hierarchyExpansionState;
            this.editorEvents = editorEvents;
        }


        public void HandleHierarchyUp()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            var aboveSibling = hierarchyOrderSystem.GetAboveSibling(selectedEntity);

            if (aboveSibling == null)
            {
                if (selectedEntity.Parent != null)
                {
                    entitySelectSystem.SelectSingle(selectedEntity.ParentId);
                }
            }
            else
            {
                if (!hierarchyExpansionState.IsExpanded(aboveSibling.Id))
                {
                    entitySelectSystem.SelectSingle(aboveSibling.Id);
                }
                else
                {
                    var aboveEntity = hierarchyOrderSystem.GetLastExpandedSuccessor(aboveSibling);
                    entitySelectSystem.SelectSingle(aboveEntity.Id);
                }
            }
        }

        public void HandleHierarchyDown()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            if (hierarchyExpansionState.IsExpanded(selectedEntity.Id))
            {
                var belowEntity = selectedEntity.Children?.OrderBy(e => e.hierarchyOrder).FirstOrDefault();

                if (belowEntity != null)
                {
                    entitySelectSystem.SelectSingle(belowEntity.Id);
                }
            }
            else
            {
                var belowSibling = hierarchyOrderSystem.GetBelowSibling(selectedEntity);

                if (belowSibling != null)
                {
                    entitySelectSystem.SelectSingle(belowSibling.Id);
                }
                else
                {
                    var belowEntity =
                        hierarchyOrderSystem.GetNextPossibleBelowSiblingOfClosestAncestor(selectedEntity);

                    if (belowEntity != null)
                    {
                        entitySelectSystem.SelectSingle(belowEntity.Id);
                    }
                }
            }
        }

        public void HandleHierarchyCollapse()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            if (!hierarchyExpansionState.IsExpanded(selectedEntity.Id))
            {
                return;
            }

            hierarchyExpansionState.ToggleExpanded(selectedEntity.Id);
            editorEvents.InvokeHierarchyChangedEvent();
        }

        public void HandleHierarchyExpand()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            if (hierarchyExpansionState.IsExpanded(selectedEntity.Id))
            {
                return;
            }

            hierarchyExpansionState.ToggleExpanded(selectedEntity.Id);
            editorEvents.InvokeHierarchyChangedEvent();
        }
    }
}
