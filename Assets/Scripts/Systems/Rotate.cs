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
    void Start()
    {
        _entity = GetComponentInParent<Entity>();
    }

    private float _snapLeftOvers;

    private float ApplySnapping(float change)
    {
        if (!SnappingManager.IsSnapping)
        {
            return change;
        }

        _snapLeftOvers += change;
        var newChange = 0f;

        while (_snapLeftOvers > SnappingManager.rotateSnapDistance / 2)
        {
            newChange += SnappingManager.rotateSnapDistance;
            _snapLeftOvers -= SnappingManager.rotateSnapDistance;
        }
        while (_snapLeftOvers < -SnappingManager.rotateSnapDistance / 2)
        {
            newChange -= SnappingManager.rotateSnapDistance;
            _snapLeftOvers += SnappingManager.rotateSnapDistance;
        }
        
        
        return newChange;
    }

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera)
    {
        const float sensitivity = 500f;

        var space = GizmoRelationManager.RelationSetting.ToSpace();

        var snappedAngle = 0f;

        switch (direction)
        {
            case RotateDirection.XAxis:
                snappedAngle = ApplySnapping(cameraSpaceChange.x * sensitivity);
                _entity.transform.Rotate(Vector3.right,snappedAngle,space);
                break;
            case RotateDirection.YAxis:
                snappedAngle = ApplySnapping(cameraSpaceChange.x * sensitivity);
                _entity.transform.Rotate(Vector3.up,snappedAngle,space);
                break;
            case RotateDirection.ZAxis:
                snappedAngle = ApplySnapping(cameraSpaceChange.x * sensitivity);
                _entity.transform.Rotate(Vector3.forward,snappedAngle,space);
                break;
            case RotateDirection.All:
                snappedAngle = ApplySnapping(cameraSpaceChange.magnitude*sensitivity);
                _entity.transform.Rotate(Vector3.Cross(globalChange,gizmoCamera.transform.forward),snappedAngle,Space.World);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    public override Plane GetPlane(Camera camera)
    {
        _snapLeftOvers = 0;

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
