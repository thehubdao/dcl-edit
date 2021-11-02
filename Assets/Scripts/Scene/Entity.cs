using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static System.Char;

public class Entity : MonoBehaviour
{
    [Serializable]
    public struct Json
    {
        public Json(Entity e)
        {
            name = e.Name;
            uniqueNumber = e.uniqueNumber;
            components = e.Components.Select(c => new EntityComponent.Json(c)).ToList();
        }
         
        public string name;
        public int uniqueNumber;
        public List<EntityComponent.Json> components;
    }
    /*
    const darkCobblestoneTile21 = new Entity('darkCobblestoneTile21')
    engine.addEntity(darkCobblestoneTile21)
    darkCobblestoneTile21.setParent(_scene)
    darkCobblestoneTile21.addComponentOrReplace(gltfShape3)
    const transform29 = new Transform({
        position: new Vector3(16, 0, 10),
        rotation: Quaternion.Euler(0,90,0),
        scale: new Vector3(1, 1, 1)
    })
    darkCobblestoneTile21.addComponentOrReplace(transform29)
    */

    public string UniqueName => Name + uniqueNumber.ToString();

    public string Name
    {
        get => _customName;
        set => _customName = value;
    }
    [SerializeField]
    private string _customName = "Entity";

    public int uniqueNumber = -1;

    public static int uniqueNumberCounter = 0;

    public string NameTsSymbol => (Name == "") ? "Entity" + uniqueNumber.ToString() : UniqueName;


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

        var script = $"const {NameTsSymbol.ToCamelCase()} = new Entity(\"{NameTsSymbol}\")\n";
        script += $"engine.addEntity({NameTsSymbol.ToCamelCase()})\n";

        var exposedVar = new ScriptGenerator.ExposedVars
        {
            exposedAs = "entity",
            symbol = NameTsSymbol.ToCamelCase()
        };
        exposed.Add(exposedVar);

        foreach (var component in Components)
        {
            var componentTs = component.GetTypeScript();
            script += componentTs.setup;
            script += $"{NameTsSymbol.ToCamelCase()}.addComponentOrReplace({componentTs.symbol})\n";

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
