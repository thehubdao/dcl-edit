using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToGround : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Rigidbody rb;

    void Start()
    {
        
    }

    private void Drop()
    {
        //do a sweep to determine distance then move object down that distance
    }
    public void OnPointerDown(PointerEventData eventData)//make button hold and release instead of toggle//tool visuals//shortcut G STRG+G//systems view 3d input system
    {
        DropWithPhysics();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        RemoveRigidBody();
    }
    private void RemoveRigidBody()
    {
        var primaryEntity = DclSceneManager.PrimarySelectedEntity;
        Destroy(primaryEntity.GetComponent(typeof(Rigidbody)));
    }
    private void DropWithPhysics()
    {
        var primaryEntity = DclSceneManager.PrimarySelectedEntity;

        rb = primaryEntity.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;


    }
}