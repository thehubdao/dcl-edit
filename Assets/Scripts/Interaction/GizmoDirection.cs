using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDirection : MonoBehaviour
{
    public bool x;
    public bool y;
    public bool z;

    public Vector3Int GetVector() => new Vector3Int(x ? 1 : 0, y ? 1 : 0, z ? 1 : 0);
}
