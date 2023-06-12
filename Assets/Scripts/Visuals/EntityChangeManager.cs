using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Zenject;

public class EntityChangeManager
{
    // Dependencies
    private ISceneManagerSystem sceneManagerSystem;
    private ExposeEntitySystem exposeEntitySystem;
    private CommandSystem commandSystem;
    private EditorEvents editorEvents;

    [Inject]
    private void Construct(ISceneManagerSystem sceneManagerSystem, ExposeEntitySystem exposeEntitySystem, CommandSystem commandSystem, EditorEvents editorEvents)
    {
        this.sceneManagerSystem = sceneManagerSystem;
        this.exposeEntitySystem = exposeEntitySystem;
        this.commandSystem = commandSystem;
        this.editorEvents = editorEvents;

        editorEvents.onEntityNameChangedEvent += OnEntityNameChanged;
    }
    

    private readonly Dictionary<Guid, SetValueByFunction<string>> hierarchyStrategies = new();
    
    public SetValueStrategy<string> GetNameForHierarchy(Guid entityId)
    {
        var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(entityId);

        // find strategy in dictionary or create new one
        if (!hierarchyStrategies.TryGetValue(entityId, out var valueStrategy))
        {
            valueStrategy = new SetValueByFunction<string>(entity.ShownName);
            hierarchyStrategies.Add(entityId, valueStrategy);
        }
        
        return valueStrategy;
    }

    private readonly Dictionary<Guid, SetValueByFunction<string>> inspectorStrategies = new();

    public SetValueStrategy<string> GetExposedNameForInspector(Guid entityId)
    {
        var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(entityId);

        // find strategy in dictionary or create new one
        if (!inspectorStrategies.TryGetValue(entityId, out var valueStrategy))
        {
            valueStrategy = new SetValueByFunction<string>($"Exposed as: {exposeEntitySystem.ExposedName(entity)}");
            inspectorStrategies.Add(entityId, valueStrategy);
        }

        return valueStrategy;
    }

    private void OnEntityNameChanged(Guid entityId)
    {
        var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(entityId);

        if (hierarchyStrategies.TryGetValue(entityId, out var valueStrategy))
        {
            valueStrategy.SetValue(entity.ShownName);
        }

        if (inspectorStrategies.TryGetValue(entityId, out valueStrategy))
        {
            valueStrategy.SetValue($"Exposed as: {exposeEntitySystem.ExposedName(entity)}");
        }
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
}
