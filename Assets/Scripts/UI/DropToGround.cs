using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropToGround : MonoBehaviour
{
    Rigidbody rb;
    Vector3 originalPosition;
    Quaternion originalRotation;
    void Start()
    {

    }

    private void Drop()
    {

    }
    public void Button()
    {
        if (DclSceneManager.PrimarySelectedEntity.TryGetComponent(out Rigidbody rb))
        {
            Reset();
        }
        else
        {
            DropWithPhysics();
        }
    }
    private void Reset()
    {
        var primaryEntity = DclSceneManager.PrimarySelectedEntity;
        Destroy(primaryEntity.GetComponent(typeof(Rigidbody)));
        primaryEntity.transform.SetPositionAndRotation(originalPosition,originalRotation);
    }
    private void DropWithPhysics()
    {
        var primaryEntity = DclSceneManager.PrimarySelectedEntity;
        originalPosition = primaryEntity.transform.position;
        originalRotation = primaryEntity.transform.rotation;
        rb=primaryEntity.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.mass = 0;
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.angularVelocity = new Vector3(0,0,0);
       // rb.isKinematic = false;
        //rb.useGravity = true;
    }
}
