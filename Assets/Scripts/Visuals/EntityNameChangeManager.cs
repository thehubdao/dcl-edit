using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

public class EntityNameChangeManager
{
    // Dependencies
    private ISceneManagerSystem sceneManagerSystem;
    private ExposeEntitySystem exposeEntitySystem;
    private CommandSystem commandSystem;

    [Inject]
    private void Construct(ISceneManagerSystem sceneManagerSystem, ExposeEntitySystem exposeEntitySystem, CommandSystem commandSystem, EditorEvents editorEvents)
    {
        this.sceneManagerSystem = sceneManagerSystem;
        this.exposeEntitySystem = exposeEntitySystem;
        this.commandSystem = commandSystem;

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
}
