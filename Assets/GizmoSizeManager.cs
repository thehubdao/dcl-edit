using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSizeManager : MonoBehaviour
{

    public void SetSize(System.Single s)
    {
        SceneManager.GizmoScale = s; 
        Debug.Log(s);
    }
}
