using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : Manager
{
    public static Camera MainCamera { get; set; }


    private static float _cameraNormalFlySpeed = 20;
    private static float _cameraFastFlySpeed = 50;
    private static float _mouseSensitivity = 3;


    private static Vector3 _position;
    private static float _pitch;
    private static float _yaw;

    public static Vector3 Position
    {
        get => _position;
        set
        {
            _position = value; 
            OnCameraMoved.Invoke();
        }
    }

    public static float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = value;

            if(_pitch>100)
                _pitch = 100;

            if(_pitch <-100)
                _pitch = -100;

            OnCameraMoved.Invoke();
        }
    }

    public static float Yaw
    {
        get => _yaw;
        set
        {
            _yaw = value;
            OnCameraMoved.Invoke();
        }
    }

    public static Quaternion Rotation => Quaternion.Euler(_pitch,_yaw,0);
    public static Vector3 Forward => Rotation * Vector3.forward;
    public static Vector3 Back => Rotation * Vector3.back;
    public static Vector3 Right => Rotation * Vector3.right;
    public static Vector3 Left => Rotation * Vector3.left;
    public static Vector3 Up => Rotation * Vector3.up;
    public static Vector3 Down => Rotation * Vector3.down;


    public static void RotateStep(Vector2 direction)
    {
        RotateStep(direction.x,direction.y);
    }

    public static void RotateStep(float directionX, float directionY)
    {
        Yaw += directionX * _mouseSensitivity;
        Pitch += directionY * -_mouseSensitivity;
    }

    public static void MoveContinuously(Vector3 localDirection,bool isFast)
    {
        MoveStep(localDirection*Time.deltaTime,isFast);
    }

    public static void MoveStep(Vector3 localDirection,bool isFast)
    {
        Position += Rotation * localDirection * (isFast?_cameraFastFlySpeed:_cameraNormalFlySpeed);
    }
    private static void MoveFixed(Vector3 localDirection)
    {
        Position += Rotation * localDirection;
    }

    public static void RotateAroundPointStep(Vector3 point, Vector2 direction)
    {
        // Find relations from position to Pivot point
        var gimbalVector = Position - point;
        var moveToPivot = Quaternion.Inverse(Rotation) * -gimbalVector;
        
        // Move to pivot
        MoveFixed(moveToPivot);
        
        // Rotate
        Yaw += direction.x * _mouseSensitivity;
        Pitch += direction.y * -_mouseSensitivity;

        // Move back from pivot
        MoveFixed(-moveToPivot);
    }


    public static UnityEvent OnCameraMoved = new UnityEvent();
}
