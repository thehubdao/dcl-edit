using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class RightClickCameraController : MonoBehaviour
{
    public float sensitivity = 1;

    public float speed = 7;
    public float sprintSpeed = 13;

    
    public void StartMovement()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EndMovement()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    public void UpdateWasdMovement()
    {
        // Rotate View
        var v = transform.InverseTransformDirection(Vector3.up);
        //Debug.DrawRay(transform.position,transform.TransformDirection(v),Color.red,0.1f);
        transform.Rotate(v, Input.GetAxis("Mouse X") * sensitivity);
        transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * -sensitivity);


        var targetMovement = Vector3.right * Input.GetAxis("Horizontal") +
                             Vector3.forward * Input.GetAxis("Vertical") +
                             Vector3.up * Input.GetAxis("UpDown");

        var isSprinting = Input.GetButton("Sprint");
        transform.Translate((isSprinting ? sprintSpeed : speed) * Time.deltaTime * targetMovement);
    }
    public void UpdateZoomMovement()
    {
        var isSprinting = Input.GetButton("Sprint");
        var targetMovement = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        transform.Translate(0,0,(isSprinting ? sprintSpeed : speed) * targetMovement*0.03f);
    }
    public void UpdateSlideMovement()
    {
        var isSprinting = Input.GetButton("Sprint");
        var targetMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        transform.Translate((isSprinting ? sprintSpeed : speed) * targetMovement*-0.03f);
    }

    public void ApplyZoom()
    {
        var isSprinting = Input.GetButton("Sprint");
        transform.Translate(0,0,Input.GetAxis("Mouse ScrollWheel")*(isSprinting ? sprintSpeed : speed));
    }

    public void UpdateRotateAroundMovement(Vector3 rotationPoint)
    {
        // Rotate View
        //var v = transform.InverseTransformDirection(Vector3.up);
        //Debug.DrawRay(transform.position,transform.TransformDirection(v),Color.red,0.1f);
        transform.RotateAround(rotationPoint,Vector3.up, Input.GetAxis("Mouse X") * sensitivity);
        transform.RotateAround(rotationPoint,transform.right, Input.GetAxis("Mouse Y") * -sensitivity);
    }
}
