using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.Command
{
    //public class AddCustomComponentToEntity : Command
    //{
    //    private Dictionary<Type, string> _properties = new Dictionary<Type, string>();
    //    private Guid _entityId;
    //    private string _name;
    //    private string _slotName;
    //
    //    public AddCustomComponentToEntity(Guid entity, string name, string slotName, params KeyValuePair<Type, string>[] properties)
    //    {
    //        _entityId = entity;
    //        _name = name;
    //        _slotName = slotName;
    //        foreach (var property in properties)
    //        {
    //            _properties.Add(property.Key, property.Value);
    //        }
    //    }
    //
    //    internal override void Do(SceneState sceneState)
    //    {
    //        var entity = sceneState.CurrentScene.GetEntityFormId(_entityId);
    //
    //        entity.Components.Add(new DclComponent(_name, _name, _properties));
    //    }
    //
    //    internal override void Undo(SceneState sceneState)
    //    {
    //        var entity = sceneState.CurrentScene.GetEntityFormId(_entityId);
    //
    //        // remove the component with the matching slot name, if one exists
    //        var component = entity.Components.Find(c => c.NameOfSlot == _slotName);
    //        if (component != null)
    //        {
    //            entity.Components.Remove(component);
    //        }
    //    }
    //}
}
