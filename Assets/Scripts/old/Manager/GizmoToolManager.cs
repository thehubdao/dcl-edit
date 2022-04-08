using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class GizmoToolManager : Manager
{
    //private static Tool _currentTool = Tool.Translate;

    [SerializeField]
    //private TextMeshProUGUI _currentManipulatorText;

    public static UnityEvent onUpdate = new UnityEvent();
    
    public enum Tool
    {
        Translate,
        Rotate,
        Scale
    }
    
    public static Tool CurrentTool
    {
        get => PersistentData.CurrentTool switch {
            0 => Tool.Translate,
            1 => Tool.Rotate,
            2 => Tool.Scale,
            _ => throw new ArgumentOutOfRangeException()
        };
        set
        {
            PersistentData.CurrentTool = value switch
            {
                Tool.Translate => 0,
                Tool.Rotate => 1,
                Tool.Scale => 2,
                _ => throw new ArgumentOutOfRangeException()
            };
            onUpdate.Invoke();
        }
    }
    
    public static void SwitchToNextManipulator()
    {
        CurrentTool = CurrentTool.Next();
    }

}
