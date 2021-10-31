using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityManipulator : MonoBehaviour
{
    public abstract void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera);
    public abstract Plane GetPlane(Camera camera);
    public abstract Ray GetOneRay();
}
