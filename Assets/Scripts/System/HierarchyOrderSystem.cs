using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class HierarchyOrderSystem
    {
        private CommandSystem commandSystem;
        private HierarchyExpansionState hierarchyExpansionState;
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        public void Construct(
            CommandSystem commandSystem, HierarchyExpansionState hierarchyExpansionState, EditorEvents editorEvents, SceneManagerSystem sceneManagersystem)
        {
            this.commandSystem = commandSystem;
            this.hierarchyExpansionState = hierarchyExpansionState;
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagersystem;
        }

        /// <summary>
        /// Gets the sibling underneath the given entity, if there is one.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The sibling, or null</returns>
        [CanBeNull]
        public DclEntity GetBelowSibling(DclEntity entity)
        {
            var parent = entity.Parent;
            List<DclEntity> children;

            if (parent == null)
            {
                var scene = sceneManagerSystem.GetCurrentScene();
                children = scene.EntitiesInSceneRoot.OrderBy(e => e.hierarchyOrder).ToList();
            }
            else
            {
                children = parent.Children.OrderBy(e => e.hierarchyOrder).ToList();
            }

            var index = children.IndexOf(entity);
            return (index + 1) >= children.Count ? null : children[index + 1];
        }
        
        /// <summary>
        /// Gets the sibling above the given entity, if there is one.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The sibling, or null</returns>
        [CanBeNull]
        public DclEntity GetAboveSibling(DclEntity entity)
        {
            var parent = entity.Parent;
            List<DclEntity> children;

            if (parent == null)
            {
                var scene = sceneManagerSystem.GetCurrentScene();
                children = scene?.EntitiesInSceneRoot.OrderBy(e => e.hierarchyOrder).ToList();
            }
            else
            {
                children = parent.Children.OrderBy(e => e.hierarchyOrder).ToList();
            }

            var index = children.IndexOf(entity);
            return (index - 1) < 0 ? null : children[index - 1];
        }

        /// <summary>
        /// Gets the new calculated hierarchy order, when placing an entity below the last root entity.
        /// </summary>
        /// <returns>The calculated hierarchy order, or null</returns>
        private float GetNewHierarchyOrderPlaceLastInRoot()
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            
            var rootEntities = scene.EntitiesInSceneRoot.ToList();
            
            return rootEntities.Count == 0 ? 0 : rootEntities.Max(e => e.hierarchyOrder) + 1;
        }

        /// <summary>
        /// Gets the new calculated hierarchy order, when placing an entity between two siblings.
        /// </summary>
        /// <param name="first">An existing entity</param>
        /// <param name="second">Another existing entity</param>
        /// <returns>The new calculated hierarchy order or the order of the entity</returns>
        public float GetHierarchyOrderPlaceBetweenSiblings(DclEntity first, DclEntity second)
        {
            return 0.5f * (first.hierarchyOrder + second.hierarchyOrder);
        }

        /// <summary>
        /// Gets the new calculated hierarchy order, when placing an entity as a child without existing siblings.
        /// </summary>
        /// <returns>The new calculated hierarchy order</returns>
        private static float GetHierarchyOrderPlaceAsOnlyChild()
        {
            return 0.0f;
        }

        /// <summary>
        /// Gets the new calculated hierarchy order, when placing an entity above an existing sibling.
        /// </summary>
        /// <param name="nextSiblingEntity">The existing sibling</param>
        /// <returns>The new calculated hierarchy order</returns>
        private static float GetHierarchyOrderPlaceAboveSibling(DclEntity nextSiblingEntity)
        {
            return nextSiblingEntity.hierarchyOrder - 1;
        }

        /// <summary>
        /// Gets the new calculated hierarchy order, when placing an entity below an existing sibling.
        /// </summary>
        /// <param name="previousSiblingEntity">The existing sibling</param>
        /// <returns>The new calculated hierarchy order</returns>
        public float GetHierarchyOrderPlaceBeneathSibling(DclEntity previousSiblingEntity)
        {
            return previousSiblingEntity.hierarchyOrder + 1;
        }


        /// <summary>
        /// Command to change the hierarchy order of an entity
        /// </summary>
        /// <param name="draggedEntity">The dragged Entity</param>
        /// <param name="hoveredEntity">The Entity, that is being dropped on</param>
        /// <param name="newHierarchyOrder">The future hierarchy order of the dragged Entity</param>
        /// <param name="newParent">The future parent of the dragged Entity</param>
        private void ChangeHierarchyOrderAsCommand(DclEntity draggedEntity, DclEntity hoveredEntity,
            float newHierarchyOrder, DclEntity newParent)
        {
            var startHierarchyOrder = draggedEntity.hierarchyOrder;
            var startParentId = draggedEntity.Parent?.Id ?? default;
            var affectedEntityId = draggedEntity.Id;
            var newParentId = newParent?.Id ?? default;
            
            //Check for non-unique hierarchy orders of siblings.
            if (newParent != null && newParent.Children.Any(e => e.hierarchyOrder.Equals(newHierarchyOrder)))
            {
                Debug.LogError("Hierarchy order must be unique!");
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }
            
            //Check that an Ancestor isn't dragged into a Descendant.
            if (hoveredEntity != null && hoveredEntity.IsDescendantOf(draggedEntity))
            {
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }

            //Check if new child of hovered entity
            if (newParent != null && hoveredEntity != null && newParentId.Equals(hoveredEntity.Id))
            {
                hierarchyExpansionState.SetExpanded(hoveredEntity.Id, true);
            }

            commandSystem.ExecuteCommand(
                commandSystem.CommandFactory.CreateChangeHierarchyOrder(affectedEntityId, startParentId, startHierarchyOrder, newHierarchyOrder, newParentId));
        }

        /// <summary>
        /// Gets the new calculated hierarchy order, when placing an entity as the last child of a given parent.
        /// </summary>
        /// <param name="parentId">Id of the parent</param>
        /// <returns>The new calculated hierarchy order</returns>
        private float GetHierarchyOrderFromParentWhenPlacedAsLastChild(Guid parentId)
        {
            var scene = sceneManagerSystem.GetCurrentScene();

            var parent = scene.GetEntityById(parentId);
            
            float newHierarchyOrder;

            if (parent.Children.Any())
            {
                var lastChild = parent.Children.OrderBy(e => e.hierarchyOrder).Last();
                newHierarchyOrder = GetHierarchyOrderPlaceBeneathSibling(lastChild);
            }
            else
            {
                newHierarchyOrder = GetHierarchyOrderPlaceAsOnlyChild();
            }

            return newHierarchyOrder;
        }
        
        /// <summary>
        /// Gets the newly calculated default hierarchy order, using the parent.
        /// This is the new highest hierarchy order in the layer the child will be placed in.
        /// </summary>
        /// <param name="parentId">Id of the parent</param>
        /// <returns>The new calculated hierarchy order</returns>
        public float GetDefaultHierarchyOrder(Guid parentId)
        {
            float newHierarchyOrder;
            
            if (parentId != default)
            {
                newHierarchyOrder = GetHierarchyOrderFromParentWhenPlacedAsLastChild(parentId);
            }
            else
            {
                newHierarchyOrder = GetNewHierarchyOrderPlaceLastInRoot();
            }

            return newHierarchyOrder;
        }
        
        /// <summary>
        /// Handles dropping an entity onto the "top-zone" of a hierarchy item game object
        /// </summary>
        /// <param name="draggedEntity">The dragged Entity</param>
        /// <param name="hoveredEntity">The Entity, that is being dropped on</param>
        /// <param name="aboveEntity">The Entity, that is sibling above the hovered Entity</param>
        public void DropUpper(DclEntity draggedEntity, DclEntity hoveredEntity, DclEntity aboveEntity)
        {
            if (draggedEntity == aboveEntity || draggedEntity == hoveredEntity)
            {
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }
            
            float newHierarchyOrder;

            var newParent = hoveredEntity?.Parent;

            if (aboveEntity == null)
            {
                newHierarchyOrder = GetHierarchyOrderPlaceAboveSibling(hoveredEntity);
            }
            else
            {
                newHierarchyOrder = GetHierarchyOrderPlaceBetweenSiblings(hoveredEntity, aboveEntity);
            }

            ChangeHierarchyOrderAsCommand(draggedEntity, hoveredEntity, newHierarchyOrder, newParent);
        }

        /// <summary>
        /// Handles dropping an entity onto the "middle-zone" of a hierarchy item game object
        /// </summary>
        /// <param name="draggedEntity">The dragged Entity</param>
        /// <param name="hoveredEntity">The Entity, that is being dropped on</param>
        public void DropMiddle(DclEntity draggedEntity, DclEntity hoveredEntity)
        {
            if (hoveredEntity == draggedEntity)
            {
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }
            
            var newHierarchyOrder = GetHierarchyOrderFromParentWhenPlacedAsLastChild(hoveredEntity.Id);
            var newParent = hoveredEntity;

            if (draggedEntity == null || hoveredEntity.IsDescendantOf(draggedEntity))
            {
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }

            ChangeHierarchyOrderAsCommand(draggedEntity, hoveredEntity, newHierarchyOrder, newParent);
        }
        /// <summary>
        /// Handles dropping an entity onto the "lower-zone" of a hierarchy item game object
        /// </summary>
        /// <param name="draggedEntity">The dragged Entity</param>
        /// <param name="hoveredEntity">The Entity, that is being dropped on</param>
        /// <param name="belowEntity">The Entity, that is the sibling below the hovered Entity</param>
        /// <param name="firstChildOfHoveredEntity">The Entity, that is the first child of the hovered Entity</param>
        /// <param name="isExpanded">Whether the hovered Entity is expanded or not</param>
        
        public void DropLower(DclEntity draggedEntity, DclEntity hoveredEntity, DclEntity belowEntity,
            DclEntity firstChildOfHoveredEntity, bool isExpanded)
        {
            float newHierarchyOrder;
            DclEntity newParent;

            if (draggedEntity == belowEntity || draggedEntity == hoveredEntity)
            {
                editorEvents.InvokeHierarchyChangedEvent();
                return;
            }
            
            if (isExpanded)
            {
                if (draggedEntity == null || (hoveredEntity != null && hoveredEntity.IsDescendantOf(draggedEntity)))
                {
                    editorEvents.InvokeHierarchyChangedEvent();
                    return;
                }

                newHierarchyOrder = GetHierarchyOrderPlaceAboveSibling(firstChildOfHoveredEntity);
                newParent = hoveredEntity;
            }
            else if (belowEntity == null)
            {
                newHierarchyOrder = GetHierarchyOrderPlaceBeneathSibling(hoveredEntity);
                newParent = hoveredEntity?.Parent;
            }
            else
            {
                newHierarchyOrder = GetHierarchyOrderPlaceBetweenSiblings(hoveredEntity, belowEntity);
                newParent = hoveredEntity?.Parent;
            }

            ChangeHierarchyOrderAsCommand(draggedEntity, hoveredEntity, newHierarchyOrder, newParent);
        }

        /// <summary>
        /// Handles dropping an entity onto the dropzone of a spacer game object
        /// </summary>
        /// <param name="draggedEntity">The dragged Entity</param>
        public void DropSpacer(DclEntity draggedEntity)
        {
            var newHierarchyOrder = GetNewHierarchyOrderPlaceLastInRoot();

            ChangeHierarchyOrderAsCommand(draggedEntity, null, newHierarchyOrder, null);
        }
    }
}
