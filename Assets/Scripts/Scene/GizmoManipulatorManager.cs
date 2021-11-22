using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GizmoManipulatorManager : Manager
{
    private static Manipulator _currentManipulator = Manipulator.Translate;

    [SerializeField]
    //private TextMeshProUGUI _currentManipulatorText;

    public static UnityEvent onUpdate = new UnityEvent();
    
    public enum Manipulator
    {
        Translate,
        Rotate,
        Scale
    }
    
    public static Manipulator CurrentManipulator
    {
        get => _currentManipulator;
        set
        {
            _currentManipulator = value;
            onUpdate.Invoke();
        }
    }
    
    public static void SwitchToNextManipulator()
    {
        CurrentManipulator = CurrentManipulator.Next();
    }

}
