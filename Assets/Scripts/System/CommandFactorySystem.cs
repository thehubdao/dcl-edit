using Assets.Scripts.Command;
using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

public class CommandFactorySystem
{
    // Change Property Command
    public ChangeProperty<T> CreateChangePropertyCommand<T>(DclPropertyIdentifier identifier, T oldValue, T newValue)
    {
        return new ChangeProperty<T>(identifier, oldValue, newValue);
    }

    // Translate Transform
    public TranslateTransform CreateTranslateTransform(Guid selectedEntity, Vector3 oldFixedPosition, Vector3 newFixedPosition)
    {
        return new TranslateTransform(selectedEntity, oldFixedPosition, newFixedPosition);
    }

    // Rotate Transform
    public RotateTransform CreateRotateTransform(Guid selectedEntity, Quaternion oldFixedRotation, Quaternion newFixedRotation)
    {
        return new RotateTransform(selectedEntity, oldFixedRotation, newFixedRotation);
    }

    // Scale Transform
    public ScaleTransform CreateScaleTransform(Guid selectedEntity, Vector3 oldFixedScale, Vector3 newFixedScale)
    {
        return new ScaleTransform(selectedEntity, oldFixedScale, newFixedScale);
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
    public DuplicateEntity CreateDuplicateEntity(Guid entityId)
    {
        return new DuplicateEntity(entityId);
    }

    public AddModelAssetToScene CreateAddModelAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene)
    {
        return new AddModelAssetToScene(entityId, entityCustomName, assetId, positionInScene);
    }

    public AddComponent CreateAddComponent(Guid entityId, DclComponent.ComponentDefinition component)
    {
        return new AddComponent(entityId, component);
    }

    public RemoveComponent CreateRemoveComponent(Guid entityId, DclComponent component)
    {
        return new RemoveComponent(entityId, component);
    }

    public AddEntity CreateAddEntity(EntityPresetState.EntityPreset preset, Guid oldPrimarySelection, IEnumerable<Guid> oldSecondarySelection, Guid parent = default)
    {
        return new AddEntity(oldPrimarySelection, oldSecondarySelection, preset, parent);
    }

    public RemoveEntity CreateRemoveEntity(DclEntity entity)
    {
        return new RemoveEntity(entity);
    }
}

