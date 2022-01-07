using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizeHierarchyOrderValues : MonoBehaviour
{
    public static void Normalize()
    {
        var root = SceneManager.SceneRoot;
        if (root == null)
            throw new Exception("EntityParent should have a SceneTreeObject");
        
        NormalizeRecursive(root);
    }

    private static void NormalizeRecursive(SceneTreeObject sto)
    {
        var currentValue = 0f;
        foreach (var child in sto.Children)
        {
            child.HierarchyOrder = currentValue++;
            NormalizeRecursive(child);
        }
    }
}
