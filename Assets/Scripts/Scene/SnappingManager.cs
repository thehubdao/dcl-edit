using System;
using UnityEngine;
using UnityEngine.Events;

public class SnappingManager : Manager
{
    public static bool IsSnapping
    {
        get => PersistentData.IsSnapping>0;
        set
        {
            PersistentData.IsSnapping = value ? 1 : 0;
            onSnappingSettingsChange.Invoke();
        }
    }


    public static float translateSnapDistance = 0.25f;
    public static float rotateSnapDistance = 15f;
    public static float scaleSnapDistance = 0.25f;

    public static UnityEvent onSnappingSettingsChange = new UnityEvent();


}
