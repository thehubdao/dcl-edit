using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToGround : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    static TransformUndo dropWithPhysicsUndo;

    public static void Drop()//Drop an item to the ground by making a sweeptest and finding the distance untill the first collision
    {
        if (DclSceneManager.PrimarySelectedEntity != null)
        {
            TransformUndo transformUndo = new TransformUndo(DclSceneManager.AllSelectedEntities);
            transformUndo.SaveBeginningState();

            var primaryEntity = DclSceneManager.PrimarySelectedEntity;
            float collisionCheckDistance = 256;//max distance for check

            Rigidbody rb = primaryEntity.GetComponent(typeof(Rigidbody)) as Rigidbody;
            if (rb == null)
            {
                rb = primaryEntity.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            }

            MeshCollider mesh = primaryEntity.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
            mesh.convex = true;
            RaycastHit hit;
            if (rb.SweepTest(new Vector3(0, -1, 0), out hit, collisionCheckDistance))//this only works with simple or convex meshes
            {
                primaryEntity.transform.position += new Vector3(0, -1, 0) * hit.distance;
            }
            Destroy(primaryEntity.GetComponent(typeof(Rigidbody)));
            Destroy(primaryEntity.GetComponent(typeof(MeshCollider)));
            DclSceneManager.OnSelectedEntityTransformChange.Invoke();

            transformUndo.SaveEndingState();
            transformUndo.AddUndoItem();

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        DropWithPhysics();
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        RemoveRigidBody();
    }

    public static void RemoveRigidBody()
    {
        DclSceneManager.OnSelectedEntityTransformChange.Invoke();
        var primaryEntity = DclSceneManager.PrimarySelectedEntity;
        Destroy(primaryEntity.GetComponent(typeof(Rigidbody)));
        dropWithPhysicsUndo.SaveEndingState();
        dropWithPhysicsUndo.AddUndoItem();
    }

    public static void DropWithPhysics()//Drop an item towards the ground by adding a Rigidbody
    {
        if (DclSceneManager.PrimarySelectedEntity != null)
        {
            dropWithPhysicsUndo = new TransformUndo(DclSceneManager.AllSelectedEntities);
            dropWithPhysicsUndo.SaveBeginningState();

            var primaryEntity = DclSceneManager.PrimarySelectedEntity;

            primaryEntity.gameObject.AddComponent(typeof(Rigidbody));

        }
    }
}

