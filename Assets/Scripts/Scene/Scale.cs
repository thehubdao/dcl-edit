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

        while (_snapLeftOvers > SnappingManager.scaleSnapDistance / 2)
        {
            newChange += SnappingManager.scaleSnapDistance;
            _snapLeftOvers -= SnappingManager.scaleSnapDistance;
        }
        while (_snapLeftOvers < -SnappingManager.scaleSnapDistance / 2)
        {
            newChange -= SnappingManager.scaleSnapDistance;
            _snapLeftOvers += SnappingManager.scaleSnapDistance;
        }
        
        
        return newChange;
    }

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera)
    {
        var localScale = _entity.componentsParent.transform.localScale;

        var snappedChange = 0f;

        switch (direction)
        {
            case TranslateDirection.XAxis:
                snappedChange = ApplySnapping(localChange.x);
                localScale.x += snappedChange;
                break;
            case TranslateDirection.YAxis:
                snappedChange = ApplySnapping(localChange.y);
                localScale.y += snappedChange;
                break;
            case TranslateDirection.ZAxis:
                snappedChange = ApplySnapping(localChange.z);
                localScale.z += snappedChange;
                break;
            case TranslateDirection.All:
                var averageValue = (localScale.x + localScale.y + localScale.z) / 3f;
                var localScaleAveragized = localScale / averageValue;
                snappedChange = ApplySnapping(cameraSpaceChange.x);
                localScale += localScaleAveragized * snappedChange;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        

        _entity.componentsParent.transform.localScale = localScale;
    }

    public override Plane GetPlane(Camera camera)
    {
        _snapLeftOvers = 0;

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
