using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //private Entity _entity;
    void Start()
    {
        //_entity = GetComponentInParent<Entity>();
    }

    private float _snapLeftOvers;

    private float ApplySnapping(float change)
    {
        var snapping =
            SnappingManager.IsSnapping ^ 
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));

        if (!snapping)
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

        var entities = SceneManager.AllSelectedEntities.Select(entity => entity.transform);
        var primaryEntity = SceneManager.PrimarySelectedEntity;

        Vector3 axis;
        switch (direction)
        {
            case RotateDirection.XAxis:
                snappedAngle = ApplySnapping(cameraSpaceChange.x * sensitivity);

                axis = GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Global ?
                    Vector3.right :
                    primaryEntity.transform.TransformDirection(Vector3.right);

                entities.Forall(entity => entity.transform.RotateAround(primaryEntity.transform.position, axis, snappedAngle));
                break;
            case RotateDirection.YAxis:
                snappedAngle = ApplySnapping(cameraSpaceChange.x * sensitivity);

                axis = GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Global ?
                    Vector3.up :
                    primaryEntity.transform.TransformDirection(Vector3.up);

                entities.Forall(entity => entity.transform.RotateAround(primaryEntity.transform.position, axis, snappedAngle));
                break;
            case RotateDirection.ZAxis:
                snappedAngle = ApplySnapping(cameraSpaceChange.x * sensitivity);
                axis = GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Global ?
                    Vector3.forward :
                    primaryEntity.transform.TransformDirection(Vector3.forward);

                entities.Forall(entity => entity.transform.RotateAround(primaryEntity.transform.position, axis, snappedAngle));
                break;
            case RotateDirection.All:
                snappedAngle = ApplySnapping(cameraSpaceChange.magnitude * sensitivity);
                entities.Forall(entity => entity.transform.RotateAround(primaryEntity.transform.position, Vector3.Cross(globalChange, gizmoCamera.transform.forward), snappedAngle));
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
        var entity = SceneManager.PrimarySelectedEntity;

        switch (direction)
        {
            case RotateDirection.XAxis:
                return new Ray(entity.transform.position, entity.transform.right);
            case RotateDirection.YAxis:
                return new Ray(entity.transform.position, entity.transform.up);
            case RotateDirection.ZAxis:
                return new Ray(entity.transform.position, entity.transform.forward);
            case RotateDirection.All:
                return new Ray(entity.transform.position, Vector3.up);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
