using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSizeManager : Manager
{
    [SerializeField]
    private static float _gizmoScale;

    public static float GizmoScale
    {
        get => _gizmoScale;
        set
        {
            _gizmoScale = value;
            
        }
    }
    

    public void SetSize(System.Single s)
    {
        //SceneManager.GizmoScale = s; 
        //Debug.Log(s);
        GizmoScale = s;
    }
}
