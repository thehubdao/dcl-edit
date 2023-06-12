using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
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
            CommandSystem commandSystem, HierarchyExpansionState hierarchyExpansionState, EditorEvents editorEvents,
            SceneManagerSystem sceneManagersystem)
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
        /// Gets the next sibling below the given entity that has children, if there is one.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The sibling, or null</returns>
        [CanBeNull]
        public DclEntity GetNextBelowSiblingWithChildren(DclEntity entity)
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

            if (index + 1 >= children.Count)
            {
                return null;
            }

            for (var i = index + 1; i < children.Count; i++)
            {
                var child = children[i];

                if (child.Children.Count() > 0)
                {
                    return child;
                }
            }

            return null;
        }


        /// <summary>
        /// Gets the previous (above) entity in the hierarchy
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The previous (above) entity in the hierarchy</returns>
        [CanBeNull]
        public DclEntity GetPreviousEntityInHierarchy([NotNull] DclEntity entity)
        {
            var aboveSibling = GetAboveSibling(entity);

            if (aboveSibling == null)
            {
                return entity.Parent;
            }

            if (!hierarchyExpansionState.IsExpanded(aboveSibling.Id))
            {
                return aboveSibling;
            }

            var aboveEntity = GetLastExpandedSuccessor(aboveSibling);

            return aboveEntity;
        }


        /// <summary>
        /// Gets the next entity (below) in the hierarchy.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The next entity in the hierarchy</returns>
        [CanBeNull]
        public DclEntity GetNextEntityInHierarchy([NotNull] DclEntity entity)
        {
            if (hierarchyExpansionState.IsExpanded(entity.Id))
            {
                var firstChild = entity.Children?.OrderBy(e => e.hierarchyOrder).FirstOrDefault();

                return firstChild;
            }

            var belowSibling = GetBelowSibling(entity);

            if (belowSibling != null)
            {
                return belowSibling;
            }

            var belowEntity = GetNextPossibleBelowSiblingOfClosestToFarthestAncestor(entity);

            return belowEntity;
        }


        /// <summary>
        /// Gets the next entity (below) in the hierarchy that has children.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The next entity in the hierarchy that has children</returns>
        [CanBeNull]
        public DclEntity GetNextEntityWithChildrenInHierarchy([NotNull] DclEntity entity)
        {
            var children = entity.Children?.OrderBy(e => e.hierarchyOrder);

            var childWithChildren = children?.FirstOrDefault(child => child.Children.Count() > 0);

            if (childWithChildren != null)
            {
                return childWithChildren;
            }

            var nextBelowSiblingWithChildren = GetNextBelowSiblingWithChildren(entity);

            if (nextBelowSiblingWithChildren != null)
            {
                return nextBelowSiblingWithChildren;
            }

            var nextBelowEntityWithChildren =
                GetNextPossibleBelowSiblingWithChildrenOfClosestToFarthestAncestor(entity);

            return nextBelowEntityWithChildren;
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
        private float GetNewHierarchyOrderPlaceLastInRoot([CanBeNull] DclEntity draggedEntity)
        {
            var scene = sceneManagerSystem.GetCurrentScene();

            var rootEntities = scene.EntitiesInSceneRoot.ToList();

            var lastEntityInRoot = rootEntities.OrderByDescending(e => e.hierarchyOrder).FirstOrDefault();

            //No existing entities in root
            if (lastEntityInRoot == null)
            {
                return 0;
            }

            //If draggedEntity already exists as last sibling, return same hierarchy order
            if (draggedEntity != null && draggedEntity.Id.Equals(lastEntityInRoot.Id))
            {
                return draggedEntity.hierarchyOrder;
            }

            return lastEntityInRoot.hierarchyOrder + 1;
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
                commandSystem.CommandFactory.CreateChangeHierarchyOrder(affectedEntityId, startParentId,
                    startHierarchyOrder, newHierarchyOrder, newParentId));
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
                newHierarchyOrder = GetNewHierarchyOrderPlaceLastInRoot(null);
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
            Assert.IsNotNull(draggedEntity);
            Assert.IsNotNull(hoveredEntity);
            Assert.AreNotEqual(draggedEntity, hoveredEntity);

            if (draggedEntity == aboveEntity)
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
            Assert.IsNotNull(draggedEntity);
            Assert.IsNotNull(hoveredEntity);
            Assert.AreNotEqual(draggedEntity, hoveredEntity);

            var newHierarchyOrder = GetHierarchyOrderFromParentWhenPlacedAsLastChild(hoveredEntity.Id);
            var newParent = hoveredEntity;

            if (hoveredEntity.IsDescendantOf(draggedEntity))
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

            Assert.IsNotNull(draggedEntity);
            Assert.IsNotNull(hoveredEntity);
            Assert.AreNotEqual(draggedEntity, hoveredEntity);

            if (isExpanded)
            {
                if (hoveredEntity.IsDescendantOf(draggedEntity))
                {
                    editorEvents.InvokeHierarchyChangedEvent();
                    return;
                }

                newHierarchyOrder = GetHierarchyOrderPlaceAboveSibling(firstChildOfHoveredEntity);
                newParent = hoveredEntity;
            }
            else if (draggedEntity == belowEntity)
            {
                editorEvents.InvokeHierarchyChangedEvent();
                return;
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
            Assert.IsNotNull(draggedEntity);
            var newHierarchyOrder = GetNewHierarchyOrderPlaceLastInRoot(draggedEntity);

            ChangeHierarchyOrderAsCommand(draggedEntity, null, newHierarchyOrder, null);
        }

        public void PlaceAbove([NotNull] DclEntity hoveredEntity)
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            var selectedEntity = scene.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null || selectedEntity.Id == hoveredEntity.Id)
            {
                return;
            }

            DropUpper(selectedEntity, hoveredEntity, GetAboveSibling(hoveredEntity));
        }

        public void PlaceBelow([NotNull] DclEntity hoveredEntity)
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            var selectedEntity = scene.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null || selectedEntity.Id == hoveredEntity.Id)
            {
                return;
            }

            DropLower(selectedEntity, hoveredEntity, GetBelowSibling(hoveredEntity), null, false);
        }

        public void PlaceAsChild([NotNull] DclEntity hoveredEntity)
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            var selectedEntity = scene.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null || selectedEntity.Id == hoveredEntity.Id)
            {
                return;
            }

            if (hierarchyExpansionState.IsExpanded(hoveredEntity.Id))
            {
                DropLower(selectedEntity, hoveredEntity, null,
                    hoveredEntity.Children.OrderBy(e => e.hierarchyOrder).FirstOrDefault(), true);
            }
            else
            {
                DropMiddle(selectedEntity, hoveredEntity);
            }
        }

        /// <summary>
        /// Get the last Successor of an Entity
        /// </summary>
        /// <param name="entity">The (included) entity to start from</param>
        /// <returns>The last Successor</returns>
        private DclEntity GetLastExpandedSuccessor([NotNull] DclEntity entity)
        {
            var newParent = entity;

            while (hierarchyExpansionState.IsExpanded(newParent.Id))
            {
                var lastChild = newParent.Children?.OrderByDescending((e) => e.hierarchyOrder).FirstOrDefault();

                if (lastChild == null)
                {
                    break;
                }

                newParent = lastChild;
            }

            return newParent;
        }

        /// <summary>
        /// Returns the next possible entity starting from a leaf node. Goes up the family tree in breadth first manner.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The next entity</returns>
        private DclEntity GetNextPossibleBelowSiblingOfClosestToFarthestAncestor([NotNull] DclEntity entity)
        {
            while (entity.Parent != null)
            {
                var nextEntity = GetBelowSibling(entity.Parent);

                if (nextEntity != null)
                {
                    return nextEntity;
                }

                entity = entity.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns the next possible entity starting from a leaf node that has children. Goes up the family tree in breadth first manner.
        /// </summary>
        /// <param name="entity">The entity to search from</param>
        /// <returns>The next entity with children</returns>
        private DclEntity GetNextPossibleBelowSiblingWithChildrenOfClosestToFarthestAncestor([NotNull] DclEntity entity)
        {
            while (entity.Parent != null)
            {
                var nextEntity = GetNextBelowSiblingWithChildren(entity.Parent);

                if (nextEntity != null)
                {
                    return nextEntity;
                }

                entity = entity.Parent;
            }

            return null;
        }
    }
}
