using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityManipulator : MonoBehaviour
{
    public abstract void Change(Vector3 change);
    public abstract Plane GetPlane(Camera camera);
    public abstract Ray GetOneRay();
}
