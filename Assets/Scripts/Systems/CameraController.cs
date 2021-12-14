using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 1;

    public float speed = 7;
    public float sprintSpeed = 13;

    
    //public static Transform mainCameraTransform;
    void Start()
    {
        //mainCameraTransform = transform;
        CameraManager.MainCamera = GetComponent<Camera>();

        CameraManager.OnCameraMoved.AddListener(SetDirty);

        SetDirty();
    }

    private bool _isDirty = false;

    public void SetDirty()
    {
        _isDirty = true;
    }

    void LateUpdate()
    {
        if (_isDirty)
        {
            _isDirty = false;
            UpdateCameraTransform();
        }
    }

    private void UpdateCameraTransform()
    {
        transform.position = CameraManager.Position;
        transform.rotation = CameraManager.Rotation;
    }
}
