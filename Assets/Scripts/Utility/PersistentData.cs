using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentData
{
    /*
    public static float SOMENAME
    {
        set => PlayerPrefs.SetFloat("SOMENAME",value);
        get => !PlayerPrefs.HasKey("SOMENAME") ? [Default value] : PlayerPrefs.GetFloat("SOMENAME");
    }
    public static int SOMENAME
    {
        set => PlayerPrefs.SetInt("SOMENAME",value);
        get => !PlayerPrefs.HasKey("SOMENAME") ? [Default value] : PlayerPrefs.GetInt("SOMENAME");
    }
    public static string SOMENAME
    {
        set => PlayerPrefs.SetString("SOMENAME",value);
        get => !PlayerPrefs.HasKey("SOMENAME") ? [Default value] : PlayerPrefs.GetString("SOMENAME");
    }
    */

    // Template: peda
    
    public static float GizmoSize
    {
        set => PlayerPrefs.SetFloat("GizmoSize", value);
        get => !PlayerPrefs.HasKey("GizmoSize") ? 0.15f : PlayerPrefs.GetFloat("GizmoSize");
    }

    public static int CurrentTool
    {
        set => PlayerPrefs.SetInt("CurrentTool", value);
        get => !PlayerPrefs.HasKey("CurrentTool") ? 0 : PlayerPrefs.GetInt("CurrentTool");
    }

    public static int GizmoRelation
    {
        set => PlayerPrefs.SetInt("GizmoRelation", value);
        get => !PlayerPrefs.HasKey("GizmoRelation") ? 1 : PlayerPrefs.GetInt("GizmoRelation");
    }

    public static int IsSnapping
    {
        set => PlayerPrefs.SetInt("IsSnapping", value);
        get => !PlayerPrefs.HasKey("IsSnapping") ? 1 : PlayerPrefs.GetInt("IsSnapping");
    }
}
