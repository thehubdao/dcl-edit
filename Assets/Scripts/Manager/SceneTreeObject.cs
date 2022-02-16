using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Transform = UnityEngine.Transform;

public abstract class SceneTreeObject : MonoBehaviour
{

    [SerializeField]
    public Transform childParent = default;

    
    [SerializeField]
    private float _hierarchyOrder = 0f;
    public float HierarchyOrder
    {
        get => _hierarchyOrder;
        set
        {
            _hierarchyOrder = value;
            SceneManager.OnUpdateHierarchy.Invoke();
        }
    }


    public abstract SceneTreeObject Parent { get; set; }

    public IEnumerable<SceneTreeObject> Children
    {
        get
        {
            var list = new SortedList<float, SceneTreeObject>();

            foreach (Transform child in childParent)
            {
                var childEntity = child.GetComponent<SceneTreeObject>();
                while (list.ContainsKey(childEntity._hierarchyOrder))
                    childEntity._hierarchyOrder += Random.Range(-0.01f, 0.01f);
                list.Add(childEntity._hierarchyOrder,childEntity);
            }

            return list.AsEnumerable().Select(pair => pair.Value);
        }
    }

    public int AllChildCount => GetComponentsInChildren<SceneTreeObject>().Length - 1;

    public SceneTreeObject LastChild => Children.Any() ? Children.Last() : null;

    public int Level{
        get
        {
            var i = 0;
            var tmp = this;
            while(tmp.Parent.GetType()!=typeof(RootSceneObject))
            {
                i++;
                tmp = tmp.Parent;
            }
            return i;
        }
    }

    public SceneTreeObject PreviousSibling 
    {
        get
        {
            SceneTreeObject lastSibling = null;
            foreach (var sibling in Parent.Children)
            {
                if (sibling._hierarchyOrder >= _hierarchyOrder)
                {
                    return lastSibling;
                }

                lastSibling = sibling;
            }

            return null;
        }
    }

    public bool CollapsedChildren
    {
        get => _collapsedChildren;
        set
        {
            _collapsedChildren = value;
            SceneManager.OnUpdateHierarchy.Invoke();
        }
    }

    private bool _collapsedChildren = false;
    
    public string GetTree(int level = 0)
    {
        var retVal = "";

        foreach (var i in Enumerable.Range(0, level))
        {
            retVal = retVal + "\t";
        }

        retVal += (this as Entity)?.ShownName ?? "Scene";
        retVal += "\n";

        foreach (var child in Children)
        {
            retVal = retVal += child.GetTree(level + 1);
        }

        return retVal;
    }
}