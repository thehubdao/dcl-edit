using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : Manager
{
    public static Camera MainCamera { get; set; }


    private static float CameraNormalFlySpeed => PersistentData.CameraSpeed;
    private static float CameraFastFlySpeed => CameraNormalFlySpeed * 3;
    private static float MouseSensitivity => PersistentData.MouseSensitivity;


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

            if (_pitch > 100)
                _pitch = 100;

            if (_pitch < -100)
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

    public static Quaternion Rotation => Quaternion.Euler(_pitch, _yaw, 0);
    public static Vector3 Forward => Rotation * Vector3.forward;
    public static Vector3 Back => Rotation * Vector3.back;
    public static Vector3 Right => Rotation * Vector3.right;
    public static Vector3 Left => Rotation * Vector3.left;
    public static Vector3 Up => Rotation * Vector3.up;
    public static Vector3 Down => Rotation * Vector3.down;

    public static void ChooseReasonableStartPosition()
    {
        // Calculate average parcel center
        var averageCenter = Vector2.zero;
        foreach (var parcel in DclSceneManager.sceneJson.scene.Parcels)
        {
            var nulledParcel = parcel - DclSceneManager.sceneJson.scene.Base;
            averageCenter += (Vector2)nulledParcel * 16 + new Vector2(8, 8);
        }

        if(DclSceneManager.sceneJson.scene.Parcels.Length>0)
            averageCenter /= DclSceneManager.sceneJson.scene.Parcels.Length;

        var averageCenterWorldPoint = new Vector3(averageCenter.x, 0, averageCenter.y);
        //Debug.DrawRay(averageCenterWorldPoint,Vector3.up,Color.red,10);

        Position = averageCenterWorldPoint;

        // Move Camera up
        Yaw = 45;
        Pitch = 30;
        
        var dist = Mathf.Log(DclSceneManager.sceneJson.scene.Parcels.Length, 2);

        if (dist < 0)
            dist = 0;

        MoveFixed(new Vector3(0, 0, -10 * (dist + 1)));

        Pitch = 45;
    }

    public static void RotateStep(Vector2 direction)
    {
        RotateStep(direction.x, direction.y);
    }

    public static void RotateStep(float directionX, float directionY)
    {
        Yaw += directionX * MouseSensitivity;
        Pitch += directionY * -MouseSensitivity;
    }

    public static void MoveContinuously(Vector3 localDirection, bool isFast)
    {
        MoveStep(localDirection * Time.deltaTime, isFast);
    }

    public static void MoveStep(Vector3 localDirection, bool isFast)
    {
        Position += Rotation * localDirection * (isFast ? CameraFastFlySpeed : CameraNormalFlySpeed);
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
        Yaw += direction.x * MouseSensitivity;
        Pitch += direction.y * -MouseSensitivity;

        // Move back from pivot
        MoveFixed(-moveToPivot);
    }


    public static UnityEvent OnCameraMoved = new UnityEvent();
}
