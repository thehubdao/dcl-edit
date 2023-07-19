using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using ModestTree;
using UnityEngine;
using Zenject;

public class EntityChangeManager
{
    // Dependencies
    private ISceneManagerSystem sceneManagerSystem;
    private ExposeEntitySystem exposeEntitySystem;
    private CommandSystem commandSystem;
    private EditorEvents editorEvents;
    private AvailableComponentsState availableComponentsState;
    private PromptSystem promptSystem;

    [Inject]
    private void Construct(
        ISceneManagerSystem sceneManagerSystem,
        ExposeEntitySystem exposeEntitySystem,
        CommandSystem commandSystem,
        EditorEvents editorEvents,
        AvailableComponentsState availableComponentsState,
        PromptSystem promptSystem)
    {
        this.sceneManagerSystem = sceneManagerSystem;
        this.exposeEntitySystem = exposeEntitySystem;
        this.commandSystem = commandSystem;
        this.editorEvents = editorEvents;
        this.availableComponentsState = availableComponentsState;
        this.promptSystem = promptSystem;
    }


    public SetValueStrategy<string> GetNameStrategy(Guid entityId)
    {
        return new SetValueStrategy<string>(() =>
        {
            var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(entityId);
            return entity.CustomName;
        });
    }

    public SetValueStrategy<string> GetExposedNameStrategy(Guid entityId)
    {
        return new SetValueStrategy<string>(() =>
        {
            var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(entityId);
            return exposeEntitySystem.ExposedName(entity);
        });
    }

    public ValueBindStrategy<string> GetNameFieldBinding(Guid id)
    {
        var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(id);

        var strategy = new ValueBindStrategy<string>(
            () => entity.CustomName,
            onValueSubmitted: value =>
                commandSystem.ExecuteCommand(
                    commandSystem.CommandFactory.CreateChangeEntityName(
                        id,
                        value,
                        entity.CustomName)));

        return strategy;
    }

    public ValueBindStrategy<bool> GetIsExposedBinding(Guid id)
    {
        var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(id);

        var strategy = new ValueBindStrategy<bool>(
            () => entity.IsExposed,
            onValueSubmitted: value =>
            {
                if (value == entity.IsExposed)
                    return;

                if (value)
                {
                    if (exposeEntitySystem.IsEntityExposable(entity))
                    {
                        commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeIsExposed(entity.Id, true, entity.IsExposed));
                    }
                    else
                    {
                        // TODO: show expose failed message
                        editorEvents.InvokeOnValueChangedEvent();
                    }
                }
                else
                {
                    commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeIsExposed(entity.Id, false, entity.IsExposed));
                }
            });

        return strategy;
    }

    public ValueBindStrategy<T> GetPropertyBinding<T>(DclPropertyIdentifier propertyIdentifier)
    {
        var concreteProperty = sceneManagerSystem.GetCurrentScene().GetPropertyFromIdentifier(propertyIdentifier).GetConcrete<T>();
        return new ValueBindStrategy<T>(
            value: () => concreteProperty.Value,
            onValueSubmitted: value =>
            {
                concreteProperty.ResetFloating();
                var oldValue = concreteProperty.Value;
                var command = commandSystem.CommandFactory.CreateChangePropertyCommand(propertyIdentifier, oldValue, value);

                commandSystem.ExecuteCommand(command);
            },
            onErrorSubmitted: _ => concreteProperty.ResetFloating(),
            onValueChanged: value => concreteProperty.SetFloatingValue(value),
            onErrorChanged: _ => concreteProperty.ResetFloating());
    }

    public LeftClickStrategy GetClickAssetStrategy(DclPropertyIdentifier propertyIdentifier)
    {
        return new LeftClickStrategy(async e =>
            {
                var selectedAsset = await promptSystem.CreateAssetMenu(propertyIdentifier);
            }
        );
    }

    public DropStrategy GetAssetDropStrategy(DclPropertyIdentifier propertyIdentifier)
    {
        var property = sceneManagerSystem.GetCurrentScene().GetPropertyFromIdentifier(propertyIdentifier);

        if (property.Type != DclComponent.DclComponentProperty.PropertyType.Asset)
        {
            throw new InvalidOperationException("Cannot create asset drop strategy for non-asset property");
        }

        var propertyDefinition = availableComponentsState
            .GetComponentDefinitionByName(propertyIdentifier.Component)
            .GetPropertyDefinitionByName(propertyIdentifier.Property);

        var dropStrategy = new DropStrategy();

        if ((propertyDefinition.flags & DclComponent.DclComponentProperty.PropertyDefinition.Flags.ModelAssets) != 0)
        {
            dropStrategy.dropModelAssetStrategy = new DropModelAssetStrategy(
                guid =>
                {
                    commandSystem.ExecuteCommand(
                        commandSystem.CommandFactory.CreateChangePropertyCommand(
                            propertyIdentifier,
                            property.GetConcrete<Guid>().FixedValue,
                            guid));
                });
        }

        if ((propertyDefinition.flags & DclComponent.DclComponentProperty.PropertyDefinition.Flags.SceneAssets) != 0)
        {
            dropStrategy.dropSceneAssetStrategy = new DropSceneAssetStrategy(
                guid =>
                {
                    commandSystem.ExecuteCommand(
                        commandSystem.CommandFactory.CreateChangePropertyCommand(
                            propertyIdentifier,
                            property.GetConcrete<Guid>().FixedValue,
                            guid));
                });
        }
        
        return dropStrategy;
    }
}
