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


    public static float TranslateSnapDistance => ProjectData.translateSnapStep;
    public static float RotateSnapDistance => ProjectData.rotateSnapStep;
    public static float ScaleSnapDistance => ProjectData.scaleSnapStep;

    public static UnityEvent onSnappingSettingsChange = new UnityEvent();


}
