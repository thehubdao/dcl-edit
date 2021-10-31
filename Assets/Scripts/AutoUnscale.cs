using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUnscale : MonoBehaviour
{
    private Transform _parent;

    void Start()
    {
        _parent = transform.parent;
    }

    void LateUpdate()
    {
        transform.localScale = _parent.localScale.Invert();
    }
}

public static class AutoUnscaleUtil
{
    public static Vector3 Invert(this Vector3 vec)
    {
        return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }
}