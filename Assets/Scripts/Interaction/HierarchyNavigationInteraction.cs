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

            var previousEntityInHierarchy = hierarchyOrderSystem.GetPreviousEntityInHierarchy(selectedEntity);

            if (previousEntityInHierarchy == null)
            {
                return;
            }

            entitySelectSystem.SelectSingle(previousEntityInHierarchy.Id);
        }

        public void HandleHierarchyDown()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            var nextEntityInHierarchy = hierarchyOrderSystem.GetNextEntityInHierarchy(selectedEntity);

            if (nextEntityInHierarchy == null)
            {
                return;
            }

            entitySelectSystem.SelectSingle(nextEntityInHierarchy.Id);
        }

        public void HandleHierarchyCollapse()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            if (hierarchyExpansionState.IsExpanded(selectedEntity.Id))
            {
                hierarchyExpansionState.ToggleExpanded(selectedEntity.Id);
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }

            if (selectedEntity.Parent == null)
            {
                return;
            }

            entitySelectSystem.SelectSingle(selectedEntity.Parent.Id);
        }

        public void HandleHierarchyExpand()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            if (!hierarchyExpansionState.IsExpanded(selectedEntity.Id))
            {
                hierarchyExpansionState.ToggleExpanded(selectedEntity.Id);
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }

            var nextEntityWithChildren = hierarchyOrderSystem.GetNextEntityWithChildrenInHierarchy(selectedEntity);

            if (nextEntityWithChildren == null)
            {
                return;
            }

            entitySelectSystem.SelectSingle(nextEntityWithChildren.Id);
        }
    }
}
