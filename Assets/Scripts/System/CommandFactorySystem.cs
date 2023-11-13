using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandFactorySystem
{
    // Change Property Command
    public ChangeProperty<T> CreateChangePropertyCommand<T>(DclPropertyIdentifier identifier, T oldValue, T newValue)
    {
        return new ChangeProperty<T>(identifier, oldValue, newValue);
    }

    // Translate Transform
    public TranslateTransform CreateTranslateTransform(List<TranslateTransform.EntityTransform> entityTransforms)
    {
        return new TranslateTransform(entityTransforms);
    }

    // Rotate Transform
    public RotateTransform CreateRotateTransform(List<RotateTransform.EntityTransform> entityTransforms)
    {
        return new RotateTransform(entityTransforms);
    }

    // Scale Transform
    public ScaleTransform CreateScaleTransform(List<ScaleTransform.EntityTransform> entityTransforms)
    {
        return new ScaleTransform(entityTransforms);
    }

    // Change Entity Name
    public ChangeEntityName CreateChangeEntityName(Guid entityId, string newName, string oldName)
    {
        return new ChangeEntityName(entityId, newName, oldName);
    }

    // Change Is Exposed
    public ChangeIsExposed CreateChangeIsExposed(Guid entityId, bool newExposedState, bool oldExposedState)
    {
        return new ChangeIsExposed(entityId, newExposedState, oldExposedState);
    }

    public ChangeSelection CreateChangeSelection(Guid oldPrimary, IEnumerable<Guid> oldSecondary, Guid newPrimary, IEnumerable<Guid> newSecondary)
    {
        return new ChangeSelection(oldPrimary, oldSecondary, newPrimary, newSecondary);
    }

    // Duplicate Entity
    public DuplicateEntity CreateDuplicateEntity(Guid entityId, float hierarchyOrder)
    {
        return new DuplicateEntity(entityId, hierarchyOrder);
    }

    public AddModelAssetToScene CreateAddModelAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene, float hierarchyOrder)
    {
        return new AddModelAssetToScene(entityId, entityCustomName, assetId, positionInScene, hierarchyOrder);
    }

    public AddEcs7ModelAssetToScene CreateAddEcs7ModelAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene, float hierarchyOrder)
    {
        return new AddEcs7ModelAssetToScene(entityId, entityCustomName, assetId, positionInScene, hierarchyOrder);
    }

    public AddSceneAssetToScene CreateAddSceneAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene, float hierarchyOrder)
    {
        return new AddSceneAssetToScene(entityId, entityCustomName, assetId, positionInScene, hierarchyOrder);
    }

    public AddComponent CreateAddComponent(Guid entityId, DclComponent.ComponentDefinition component)
    {
        return new AddComponent(entityId, component);
    }

    public RemoveComponent CreateRemoveComponent(Guid entityId, DclComponent component)
    {
        return new RemoveComponent(entityId, component);
    }

    public AddEntity CreateAddEntity(EntityPresetState.EntityPreset preset, Guid oldPrimarySelection,
        IEnumerable<Guid> oldSecondarySelection, float hierarchyOrder, Guid parent = default)
    {
        return new AddEntity(oldPrimarySelection, oldSecondarySelection, preset, hierarchyOrder, parent);
    }

    public RemoveEntity CreateRemoveEntity(DclEntity entity)
    {
        return new RemoveEntity(entity);
    }

    public ChangeHierarchyOrder CreateChangeHierarchyOrder(Guid affectedEntityId, Guid startParentId, float startHierarchyOrder, float newHierarchyOrder, Guid newParentId)
    {
        return new ChangeHierarchyOrder(affectedEntityId, startParentId, startHierarchyOrder, newHierarchyOrder, newParentId);
    }
}
