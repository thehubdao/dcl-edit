using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateCap : MonoBehaviour
{
    void Start()
    {
        SetFramerate();
    }

    public static void SetFramerate()
    {
        if (PersistentData.FramerateCap <= 0)
        {
            Application.targetFrameRate = -1;
        }
        else if (PersistentData.FramerateCap != Application.targetFrameRate)
        {
            Application.targetFrameRate = PersistentData.FramerateCap;
        }
    }
}
