using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.Rendering;
using static System.Char;

public class Entity : MonoBehaviour
{
    [Serializable]
    public struct Json
    {
        public Json(Entity e)
        {
            name = e.customName;
            uniqueNumber = e.uniqueNumber;
            exposed = e.Exposed;
            components = e.Components.Select(c => new EntityComponent.Json(c)).ToList();
        }
         
        public string name;
        public int uniqueNumber;
        public bool exposed;
        public List<EntityComponent.Json> components;
    }

    // Names

    [Obsolete("you probably don't want to use this",true)]
    [NonSerialized]
    public new string name;
    
    /// <summary>
    /// The name, that is used in the TypeScript constructor for the Entity.
    /// </summary>
    /// Might contain spaces and special characters.
    /// Might not be Unique.
    public string ShownName
    {
        get => customName!=""?customName:DefaultName;
        //set => customName = value;
    }

    /// <summary>
    /// The name, that is set by the User.
    /// </summary>
    /// Might contain spaces and special characters.
    /// Might not be Unique.
    /// Might be Empty.
    [SerializeField]
    public string customName = "";

    /// <summary>
    /// The name, that is used as default 
    /// </summary>
    /// Is always "Entity"
    private const string DefaultName = "Entity";

    /// <summary>
    /// The Symbol, that is used in the internal TypeScript
    /// </summary>
    /// Is always Unique
    public string InternalSymbol => (ShownName + uniqueNumber.ToString()).ToCamelCase();

    /// <summary>
    /// The Symbol, that is exposed to the users TypeScript.
    /// </summary>
    /// Must be unique. This has to be made sure by the user.
    /// 
    /// Example:
    /// scene.mySuperCoolEntity.transform
    ///       `-- this part --´
    public string ExposedSymbol => ShownName.ToCamelCase();
    
    public int uniqueNumber = -1;
    
    public static int uniqueNumberCounter = 0;

    [SerializeField]
    private bool _exposed = false;
    public bool Exposed
    {
        get => _exposed;
        set
        {
            if (!TrySetExpose(value))
            {
                throw new IndexOutOfRangeException("Entity could not be exposed, because the symbol already exists");
            }
        }
    }

    public bool TrySetExpose(bool value)
    {
        if (!value)
        {
            _exposed = false;
            return true;
        } //else value == true

        // build list of all already existing exposedSymbols
        var allExposedSymbols = new List<string>();
        foreach (var entity in SceneManager.Entities)
        {
            if (entity.Exposed)
            {
                allExposedSymbols.Add(entity.ExposedSymbol);
            }
        }

        // Check if exposedSymbol already exists
        if (allExposedSymbols.Contains(ExposedSymbol))
        {
            return false;
        } //else

        _exposed = true;
        return true;
    }
    


    [Space]
    public GameObject gizmos;
    public GameObject componentsParent;

    

    public EntityComponent[] Components => GetComponents<EntityComponent>();

    
    /// <summary>
    /// will be true, when the game object is to be destroyed
    /// </summary>
    [NonSerialized]
    public bool doomed = false;

    void Start()
    {
        if (uniqueNumber < 0)
            uniqueNumber = uniqueNumberCounter++;
    }
    

    //private static int nameFillCount = 0;
    public string GetTypeScript(ref List<ScriptGenerator.ExposedVars> exposed)
    {
        //if (Name == "")
        //{
        //    Name = "Entity " + nameFillCount++;
        //}
        //var usedName = (Name == "") ? "Entity" + uniqueNumber.ToString() : UniqueName;

        var script = $"const {InternalSymbol} = new Entity(\"{ShownName}\")\n";
        script += $"engine.addEntity({InternalSymbol})\n";

        var exposedVar = new ScriptGenerator.ExposedVars
        {
            exposedAs = "entity",
            symbol = InternalSymbol
        };
        exposed.Add(exposedVar);

        foreach (var component in Components)
        {
            var componentTs = component.GetTypeScript();
            script += componentTs.setup;
            script += $"{InternalSymbol}.addComponentOrReplace({componentTs.symbol})\n";

            var exposedComponentVar = new ScriptGenerator.ExposedVars
            {
                exposedAs = component.ComponentName,
                symbol = componentTs.symbol
            };
            exposed.Add(exposedComponentVar);

        }


        return script;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(new Json(this));
    }
}

public static class CamelCase
{

    public static string ToCamelCase(this string s)
    {
        var retVal = "";
        bool lastWasSpace = false;

        foreach (var c in s)
        {
            if (IsLetterOrDigit(c))
            {
                retVal += lastWasSpace? ToUpper(c) :ToLower(c);
                lastWasSpace = false;
            }
            else if (IsWhiteSpace(c))
            {
                lastWasSpace = true;
            }
        }

        return retVal;
    }
}
