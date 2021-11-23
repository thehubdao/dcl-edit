using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSizeManager : Manager
{
    public static float GizmoScale
    {
        get => PersistentData.GizmoSize;
        set => PersistentData.GizmoSize = value;
    }
    

    public void SetSize(System.Single s)
    {
        //SceneManager.GizmoScale = s; 
        //Debug.Log(s);
        GizmoScale = s;
    }
}
