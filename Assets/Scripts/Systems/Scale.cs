using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var entities = SceneManager.AllSelectedEntities;
        
        var snappedChange = 0f;

        switch (direction)
        {
            case TranslateDirection.XAxis:
                snappedChange = ApplySnapping(localChange.x);
                foreach (var entity in entities)
                {
                    var localScale = entity.transform.localScale;
                    localScale.x += snappedChange;
                    entity.transform.localScale = localScale;
                }
                break;
            case TranslateDirection.YAxis:
                snappedChange = ApplySnapping(localChange.y);
                foreach (var entity in entities)
                {
                    var localScale = entity.transform.localScale;
                    localScale.y += snappedChange;
                    entity.transform.localScale = localScale;
                }
                break;
            case TranslateDirection.ZAxis:
                snappedChange = ApplySnapping(localChange.z);
                foreach (var entity in entities)
                {
                    var localScale = entity.transform.localScale;
                    localScale.z += snappedChange;
                    entity.transform.localScale = localScale;
                }
                break;
            case TranslateDirection.All:
                snappedChange = ApplySnapping(cameraSpaceChange.x);
                foreach (var entity in entities)
                {
                    var localScale = entity.transform.localScale;
                    var averageValue = (localScale.x + localScale.y + localScale.z) / 3f;
                    var localScaleAveragized = localScale / averageValue;
                    localScale += localScaleAveragized * snappedChange;
                    entity.transform.localScale = localScale;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }




    }

    public override Plane GetPlane(Camera camera)
    {
        _snapLeftOvers = 0;
        var entity = SceneManager.PrimarySelectedEntity;

        switch (direction)
        {
            case TranslateDirection.XAxis:
                return new Plane(Vector3.Project(camera.transform.position - entity.transform.position, transform.right) + entity.transform.position - camera.transform.position, entity.transform.position);
            //return new Plane(Vector3.Cross(camera.transform.up,_entity.right) , _entity.position);
            case TranslateDirection.YAxis:
                return new Plane(Vector3.Project(camera.transform.position - entity.transform.position, transform.up) + entity.transform.position - camera.transform.position, entity.transform.position);
            //return new Plane(Vector3.Cross(camera.transform.right,_entity.up) , _entity.position);
            case TranslateDirection.ZAxis:
                return new Plane(Vector3.Project(camera.transform.position - entity.transform.position, transform.forward) + entity.transform.position - camera.transform.position, entity.transform.position);
            //return new Plane(Vector3.Cross(camera.transform.up,_entity.forward) , _entity.position);
            case TranslateDirection.All:
                return new Plane(camera.transform.forward, entity.transform.position);
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public override Ray GetOneRay()
    {
        var entity = SceneManager.PrimarySelectedEntity;
        switch (direction)
        {
            case TranslateDirection.XAxis:
                return new Ray(entity.transform.position, entity.transform.right);
            case TranslateDirection.YAxis:
                return new Ray(entity.transform.position, entity.transform.up);
            case TranslateDirection.ZAxis:
                return new Ray(entity.transform.position, entity.transform.forward);
            case TranslateDirection.All:
                return new Ray(entity.transform.position, Vector3.up);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
