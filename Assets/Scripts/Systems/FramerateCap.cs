using UnityEngine;

public static class FramerateCap
{
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