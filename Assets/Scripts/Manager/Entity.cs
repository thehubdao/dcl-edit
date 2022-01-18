using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static System.Char;

public class Entity : SceneTreeObject
{
    [Serializable]
    public class Json
    {
        public Json(Entity e)
        {
            hierarchyOrder = e.HierarchyOrder;
            name = e.CustomName;
            uniqueNumber = e.uniqueNumber;
            parent = (e.Parent as Entity)?.uniqueNumber ?? -1;
            exposed = e.Exposed;
            collapsedChildren = e.CollapsedChildren;
            components = e.Components.Select(c => new EntityComponent.Json(c)).ToList();
        }

        public float hierarchyOrder;
        public string name;
        public int uniqueNumber;
        public int parent;
        public bool exposed;
        public bool collapsedChildren;
        public List<EntityComponent.Json> components;
    }

    public override SceneTreeObject Parent
    {
        get => transform.parent.GetComponentInParent<SceneTreeObject>();
        set
        {
            transform.parent = value.childParent;
            SceneManager.OnUpdateHierarchy.Invoke();
        }
    }

    // Names

    [Obsolete("you probably don't want to use this", true)]
    [NonSerialized]
    public new string name;

    /// <summary>
    /// The name, that is used in the TypeScript constructor for the Entity.
    /// </summary>
    /// Might contain spaces and special characters.
    /// Might not be Unique.
    public string ShownName
    {
        get => CustomName != "" ? CustomName : DefaultName;
        //set => customName = value;
    }

    /// <summary>
    /// The name, that is set by the User.
    /// </summary>
    /// Might contain spaces and special characters.
    /// Might not be Unique.
    /// Might be Empty.
    public string CustomName
    {
        get => _customName;
        set
        {
            _customName = value;
            ReevaluateExposeStatus();
        }
    }

    [SerializeField]
    private string _customName = "";

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

    public static int uniqueNumberCounter = 1;

    [SerializeField]
    private bool _exposed = false;
    public bool Exposed
    {
        get => _exposed;
        set
        {
            _wantsToBeExposed = value;
            ReevaluateExposeStatus();
        }
    }

    private bool _wantsToBeExposed = false;
    public bool WantsToBeExposed
    {
        get => _wantsToBeExposed;
        set
        {
            _wantsToBeExposed = value;
            ReevaluateExposeStatus();
        }
    }


    public bool ExposeFailed => _wantsToBeExposed ^ _exposed;

    private void ReevaluateExposeStatus()
    {
        if (!_wantsToBeExposed)
        {
            _exposed = false;
        }
        else // => _wantsToBeExposed == true
        {
            // build list of all already existing exposedSymbols
            var allExposedSymbols = new List<string>();
            foreach (var entity in SceneManager.Entities)
            {
                if (entity == this)
                    continue;

                if (entity.Exposed)
                {
                    allExposedSymbols.Add(entity.ExposedSymbol);
                }
            }

            // Check if exposedSymbol already exists
            if (allExposedSymbols.Contains(ExposedSymbol))
            {
                _exposed = false;
            }
            else // => exposedSymbol does not exist yet
            {
                _exposed = true;
            }
        }
        SceneManager.OnUpdateSelection.Invoke();
    }



    [Space]
    public GameObject componentsParent;


    public IEnumerable<EntityComponent> Components
    {
        get
        {
            var entityComponents = GetComponents<EntityComponent>().ToList();
            entityComponents.Sort((left, right) => left.InspectorOrder.CompareTo(right.InspectorOrder));
            return entityComponents.AsEnumerable();
        }
    }



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

    public string ToJson()
    {
        return JsonUtility.ToJson(new Json(this));
    }
}

public static class EntityUtils
{

    public static string TryGetShownName(this Entity e)
    {
        return e == null ? "nothing" : e.ShownName;
    }

    public static string ToCamelCase(this string s)
    {
        var retVal = "";
        bool lastWasSpace = false;

        foreach (var c in s)
        {
            if (IsLetterOrDigit(c))
            {
                retVal += lastWasSpace ? ToUpper(c) : ToLower(c);
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
