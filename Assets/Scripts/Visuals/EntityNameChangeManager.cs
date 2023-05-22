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
    private EditorEvents editorEvents;

    [Inject]
    private void Construct(ISceneManagerSystem sceneManagerSystem, EditorEvents editorEvents)
    {
        this.sceneManagerSystem = sceneManagerSystem;
        this.editorEvents = editorEvents;

        editorEvents.onEntityNameChangedEvent += OnEntityNameChanged;
    }
    

    private readonly Dictionary<Guid, SetValueByFunction<string>> strategies = new();
    
    public ValueStrategy<string> GetNameForId(Guid entityId)
    {
        var entity = sceneManagerSystem.GetCurrentScene().GetEntityById(entityId);

        // find strategy in dictionary or create new one
        if (!strategies.TryGetValue(entityId, out var valueStrategy))
        {
            valueStrategy = new SetValueByFunction<string>(entity.ShownName);
            strategies.Add(entityId, valueStrategy);
        }
        
        return valueStrategy;
    }
    
    private void OnEntityNameChanged(Guid entityId)
    {
        if (strategies.TryGetValue(entityId, out var valueStrategy))
        {
            valueStrategy.SetValue(sceneManagerSystem.GetCurrentScene().GetEntityById(entityId).ShownName);
        }
    }
}
