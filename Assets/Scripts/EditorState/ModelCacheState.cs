using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCacheState : MonoBehaviour
{
    public Dictionary<string, GameObject> ModelCache = new Dictionary<string, GameObject>();

    public Dictionary<string, List<Action<GameObject>>> CurrentlyLoading =
        new Dictionary<string, List<Action<GameObject>>>();
}
