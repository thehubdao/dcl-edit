using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToGround : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static void Drop()
    {
        if (DclSceneManager.PrimarySelectedEntity != null)
        {
            var primaryEntity = DclSceneManager.PrimarySelectedEntity;
            float distanceToCollision;
            float collisionCheckDistance = 256;
            Rigidbody rb = primaryEntity.GetComponent(typeof(Rigidbody)) as Rigidbody;
            if (rb == null)
            {
                rb = primaryEntity.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            }
            MeshCollider mesh= primaryEntity.gameObject.AddComponent(typeof(MeshCollider))as MeshCollider;
            mesh.convex = true;
            RaycastHit hit;
            if (rb.SweepTest(new Vector3(0, -1, 0), out hit, collisionCheckDistance))//this only works with simple or convex meshes
            {
                primaryEntity.transform.position += new Vector3(0, -1, 0) * hit.distance;
            }
            Destroy(primaryEntity.GetComponent(typeof(Rigidbody)));
            Destroy(primaryEntity.GetComponent(typeof(MeshCollider)));

        }
    }

    public void OnPointerDown(PointerEventData eventData)//make button hold and release instead of toggle//tool visuals//shortcut G STRG+G//systems view 3d input system
    {
        DropWithPhysics();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RemoveRigidBody();
    }

    public static void RemoveRigidBody()
    {
        var primaryEntity = DclSceneManager.PrimarySelectedEntity;
        Destroy(primaryEntity.GetComponent(typeof(Rigidbody)));
    }

    public static void DropWithPhysics()
    {
        if (DclSceneManager.PrimarySelectedEntity != null)
        {
            var primaryEntity = DclSceneManager.PrimarySelectedEntity;

            Rigidbody rb = primaryEntity.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        }
    }
}