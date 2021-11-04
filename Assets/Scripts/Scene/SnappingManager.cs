using System;
using UnityEngine;
using UnityEngine.Events;

public class SnappingManager : MonoBehaviour
{
    private static bool _isSnapping = true;
    public static bool IsSnapping
    {
        get => _isSnapping;
        set
        {
            _isSnapping = value; 
            onSnappingSettingsChange.Invoke();
        }
    }


    public static float translateSnapDistance = 0.25f;
    public static float rotateSnapDistance = 15f;
    public static float scaleSnapDistance = 0.25f;

    public static UnityEvent onSnappingSettingsChange = new UnityEvent();


}
