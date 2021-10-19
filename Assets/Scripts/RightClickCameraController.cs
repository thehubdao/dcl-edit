using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class RightClickCameraController : MonoBehaviour
{
    public float sensitivity = 1;

    public float speed = 7;
    public float sprintSpeed = 10;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool isMouseLocked = false;
    private Vector2 mouseLockPosition = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton((int) MouseButton.RightMouse))
        {
            if(!isMouseLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                isMouseLocked = true;
            }

            // Rotate View
            var v = transform.InverseTransformDirection(Vector3.up);
            //Debug.DrawRay(transform.position,transform.TransformDirection(v),Color.red,0.1f);
            transform.Rotate(v,Input.GetAxis("Mouse X")*sensitivity);
            transform.Rotate( Vector3.right,Input.GetAxis("Mouse Y")*-sensitivity);


            var targetMovement = Vector3.right * Input.GetAxis("Horizontal") +
                                 Vector3.forward * Input.GetAxis("Vertical")+
                                 Vector3.up * Input.GetAxis("UpDown");

            var isSprinting = Input.GetButton("Sprint");
            transform.Translate((isSprinting?sprintSpeed:speed) * Time.deltaTime * targetMovement);

        }
        else
        {
            if(isMouseLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                isMouseLocked = false;
            }
        }
    }
}
