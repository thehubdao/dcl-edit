using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSeconds : MonoBehaviour
{
    public static IEnumerator Wait(float seconds)
    {
        var startTime = Time.realtimeSinceStartup;
        while (startTime + seconds > Time.realtimeSinceStartup)
        {
            yield return null;
        }
    }
}
