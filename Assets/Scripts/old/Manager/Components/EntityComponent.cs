using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class EntityComponent: MonoBehaviour
{
    [Serializable]
    public class Json
    {
        public Json(EntityComponent ec)
        {
            name = ec.ComponentName;
            specifics = ec.SpecificJson;
        }

        public string name;
        public string specifics;
    }

    public abstract string SpecificJson { get; }
    public abstract void ApplySpecificJson(string jsonString);

    [NonSerialized]
    public Entity entity;
    
    [NonSerialized]
    protected GameObject componentRepresentation;

    public void OnDestroy()
    { 
        if(componentRepresentation!=null)
        {
            Destroy(componentRepresentation);
            DclSceneManager.OnUpdateSelection.Invoke();
        }
    }

    public virtual void Start()
    {
        entity = GetComponent<Entity>();
    }

    public abstract GameObject UiItemTemplate { get; }

    public abstract string ComponentName { get; }


    /// <summary>
    /// The order in the Inspector. Small values appear further up
    /// </summary>
    ///
    /// -100 => transform
    /// 100 => shapes
    /// 
    public abstract int InspectorOrder { get; }

    public string InternalComponentSymbol => entity.InternalSymbol + ComponentName;

    public struct Ts
    {
        public Ts(string symbol, string setup)
        {
            this.symbol = symbol;
            this.setup = setup;
        }

        public string symbol;
        public string setup;
    }

    public abstract Ts? GetTypeScript();
    //{
    //    return new Ts( $"{entityName}setup", $"const {entityName}setup = new BoxShape()\n");
    //}
}
