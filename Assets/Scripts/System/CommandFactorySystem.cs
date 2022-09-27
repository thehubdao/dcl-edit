using Assets.Scripts.Command;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CommandFactorySystem
{
    // Dependencies
    private EditorEvents _editorEvents;

    [Inject]
    private void Construct(EditorEvents editorEvents)
    {
        _editorEvents = editorEvents;
    }

    // Change Property Command
    public ChangeProperty<T> CreateChangePropertyCommand<T>(DclPropertyIdentifier identifier, T oldValue, T newValue)
    {
        return new ChangeProperty<T>(identifier, oldValue, newValue, _editorEvents);
    }

    // Translate Transform
    public TranslateTransform CreateTranslateTransform(Guid selectedEntity, Vector3 oldFixedPosition, Vector3 newFixedPosition)
    {
        return new TranslateTransform(selectedEntity, oldFixedPosition, newFixedPosition, _editorEvents);
    }

    // Rotate Transform
    public RotateTransform CreateRotateTransform(Guid selectedEntity, Quaternion oldFixedRotation, Quaternion newFixedRotation)
    {
        return new RotateTransform(selectedEntity, oldFixedRotation, newFixedRotation, _editorEvents);
    }

    // Scale Transform
    public ScaleTransform CreateScaleTransform(Guid selectedEntity, Vector3 oldFixedScale, Vector3 newFixedScale)
    {
        return new ScaleTransform(selectedEntity, oldFixedScale, newFixedScale, _editorEvents);
    }

    // Change Entity Name
    public ChangeEntityName CreateChangeEntityName(Guid entityId, string newName, string oldName)
    {
        return new ChangeEntityName(entityId, newName, oldName, _editorEvents);
    }

    // Change Is Exposed
    public ChangeIsExposed CreateChangeIsExposed(Guid entityId, bool newExposedState, bool oldExposedState)
    {
        return new ChangeIsExposed(entityId, newExposedState, oldExposedState, _editorEvents);
    }

    public ChangeSelection CreateChangeSelection(Guid oldPrimary, IEnumerable<Guid> oldSecondary, Guid newPrimary, IEnumerable<Guid> newSecondary)
    {
        return new ChangeSelection(oldPrimary, oldSecondary, newPrimary, newSecondary, _editorEvents);
    }
}