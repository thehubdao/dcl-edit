using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : EntityManipulator
{
    public enum RotateDirection
    {
        XAxis,
        YAxis,
        ZAxis,
        All
    }

    public RotateDirection direction;

    private Entity _entity;
    private GizmoRelationManager _gizmoRelationManager;
    void Start()
    {
        _entity = GetComponentInParent<Entity>();
        _gizmoRelationManager = GetComponentInParent<GizmoRelationManager>();
    }

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera)
    {
        const float sensitivity = 500f;

        var space = _gizmoRelationManager.relationSetting.ToSpace();

        switch (direction)
        {
            case RotateDirection.XAxis:
                _entity.transform.Rotate(Vector3.right,cameraSpaceChange.x*sensitivity,space);
                break;
            case RotateDirection.YAxis:
                _entity.transform.Rotate(Vector3.up,cameraSpaceChange.x*sensitivity,space);
                break;
            case RotateDirection.ZAxis:
                _entity.transform.Rotate(Vector3.forward,cameraSpaceChange.x*sensitivity,space);
                break;
            case RotateDirection.All:
                _entity.transform.Rotate(Vector3.Cross(globalChange,gizmoCamera.transform.forward),cameraSpaceChange.magnitude*sensitivity,Space.World);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    public override Plane GetPlane(Camera camera)
    {
        return new Plane(camera.transform.forward, camera.transform.position + camera.transform.forward);
        
        //switch (direction)
        //{
        //    case RotateDirection.XAxis:
        //        return new Plane(Vector3.Project(camera.transform.position-_entity.transform.position,transform.right)+_entity.transform.position-camera.transform.position , _entity.transform.position);
        //        //return new Plane(Vector3.Cross(camera.transform.up,_entity.right) , _entity.position);
        //    case RotateDirection.YAxis:
        //        return new Plane(Vector3.Project(camera.transform.position-_entity.transform.position,transform.up)+_entity.transform.position-camera.transform.position , _entity.transform.position);
        //        //return new Plane(Vector3.Cross(camera.transform.right,_entity.up) , _entity.position);
        //    case RotateDirection.ZAxis:
        //        return new Plane(Vector3.Project(camera.transform.position-_entity.transform.position,transform.forward)+_entity.transform.position-camera.transform.position , _entity.transform.position);
        //        //return new Plane(Vector3.Cross(camera.transform.up,_entity.forward) , _entity.position);
        //    case RotateDirection.All:
        //        return new Plane(camera.transform.forward, _entity.transform.position);
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}

    }

    public override Ray GetOneRay()
    {
        switch (direction)
        {
            case RotateDirection.XAxis:
                return new Ray(_entity.transform.position, _entity.transform.right);
            case RotateDirection.YAxis:
                return new Ray(_entity.transform.position, _entity.transform.up);
            case RotateDirection.ZAxis:
                return new Ray(_entity.transform.position, _entity.transform.forward);
            case RotateDirection.All:
                return new Ray(_entity.transform.position, Vector3.up);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
