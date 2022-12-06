using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SceneState;
using UnityEngine;

public class SelectionUtility : MonoBehaviour
{
    public struct SelectionWrapper
    {
        public Guid Primary;
        public List<Guid> Secondary;

        public SelectionWrapper(Guid primary, List<Guid> secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }
    }

    public static void SetSelection(DclScene scene, SelectionWrapper selection)
    {
        SetSelection(scene, selection.Primary, selection.Secondary);
    }

    public static void SetSelection(DclScene scene, Guid primarySelection, List<Guid> secondarySelection = null)
    {
        scene.SelectionState.PrimarySelectedEntity = scene.GetEntityById(primarySelection);
        scene.SelectionState.SecondarySelectedEntities.Clear();

        if (secondarySelection != null)
        {
            foreach (var secondary in secondarySelection)
            {
                scene.SelectionState.SecondarySelectedEntities.Add(scene.GetEntityById(secondary));
            }
        }
    }

    public static SelectionWrapper GetCurrentSelection(DclScene scene)
    {
        return new SelectionWrapper
        {
            Primary = scene.SelectionState.PrimarySelectedEntity.Id,
            Secondary = scene.SelectionState.SecondarySelectedEntities.Select(e => e.Id).ToList()
        };
    }
}