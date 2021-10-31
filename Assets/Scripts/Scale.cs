using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : EntityManipulator
{
    public enum TranslateDirection
    {
        XAxis,
        YAxis,
        ZAxis,
        All
    }

    public TranslateDirection direction;

    private Entity _entity;
    private GizmoRelationManager _gizmoRelationManager;
    void Start()
    {
        _entity = GetComponentInParent<Entity>();
        _gizmoRelationManager = GetComponentInParent<GizmoRelationManager>();
    }

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera)
    {
        var localScale = _entity.componentsParent.transform.localScale;

        switch (direction)
        {
            case TranslateDirection.XAxis:
                localScale.x += localChange.x;
                break;
            case TranslateDirection.YAxis:
                localScale.y += localChange.y;
                break;
            case TranslateDirection.ZAxis:
                localScale.z += localChange.z;
                break;
            case TranslateDirection.All:
                var averageValue = (localScale.x + localScale.y + localScale.z) / 3f;
                var localScaleAveragized = localScale / averageValue;
                localScale += localScaleAveragized * cameraSpaceChange.x;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        

        _entity.componentsParent.transform.localScale = localScale;
    }

    public override Plane GetPlane(Camera camera)
    {
        switch (direction)
        {
            case TranslateDirection.XAxis:
                return new Plane(Vector3.Project(camera.transform.position-_entity.transform.position,transform.right)+_entity.transform.position-camera.transform.position , _entity.transform.position);
                //return new Plane(Vector3.Cross(camera.transform.up,_entity.right) , _entity.position);
            case TranslateDirection.YAxis:
                return new Plane(Vector3.Project(camera.transform.position-_entity.transform.position,transform.up)+_entity.transform.position-camera.transform.position , _entity.transform.position);
                //return new Plane(Vector3.Cross(camera.transform.right,_entity.up) , _entity.position);
            case TranslateDirection.ZAxis:
                return new Plane(Vector3.Project(camera.transform.position-_entity.transform.position,transform.forward)+_entity.transform.position-camera.transform.position , _entity.transform.position);
                //return new Plane(Vector3.Cross(camera.transform.up,_entity.forward) , _entity.position);
            case TranslateDirection.All:
                return new Plane(camera.transform.forward, _entity.transform.position);
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public override Ray GetOneRay()
    {
        switch (direction)
        {
            case TranslateDirection.XAxis:
                return new Ray(_entity.transform.position, _entity.transform.right);
            case TranslateDirection.YAxis:
                return new Ray(_entity.transform.position, _entity.transform.up);
            case TranslateDirection.ZAxis:
                return new Ray(_entity.transform.position, _entity.transform.forward);
            case TranslateDirection.All:
                return new Ray(_entity.transform.position, Vector3.up);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
