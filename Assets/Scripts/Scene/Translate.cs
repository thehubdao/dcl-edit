using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate : EntityManipulator
{
    public enum TranslateDirection
    {
        XAxis,
        YAxis,
        ZAxis,
        XYPlane,
        YZPlane,
        ZXPlane,
        All
    }

    public TranslateDirection direction;

    private Transform _entity;
    private GizmoRelationManager _gizmoRelationManager;
    void Start()
    {
        _entity = GetComponentInParent<Entity>().transform;
        _gizmoRelationManager = GetComponentInParent<GizmoRelationManager>();
    }

    private Vector3 _snapLeftOvers;

    private Vector3 ApplySnapping(Vector3 change)
    {
        if (!SnappingManager.IsSnapping)
        {
            return change;
        }

        _snapLeftOvers += change;
        var newChange = Vector3.zero;

        while (_snapLeftOvers.x > SnappingManager.translateSnapDistance / 2)
        {
            newChange.x += SnappingManager.translateSnapDistance;
            _snapLeftOvers.x -= SnappingManager.translateSnapDistance;
        }
        while (_snapLeftOvers.x < -SnappingManager.translateSnapDistance / 2)
        {
            newChange.x -= SnappingManager.translateSnapDistance;
            _snapLeftOvers.x += SnappingManager.translateSnapDistance;
        }

        while (_snapLeftOvers.y > SnappingManager.translateSnapDistance / 2)
        {
            newChange.y += SnappingManager.translateSnapDistance;
            _snapLeftOvers.y -= SnappingManager.translateSnapDistance;
        }
        while (_snapLeftOvers.y < -SnappingManager.translateSnapDistance / 2)
        {
            newChange.y -= SnappingManager.translateSnapDistance;
            _snapLeftOvers.y += SnappingManager.translateSnapDistance;
        }

        while (_snapLeftOvers.z > SnappingManager.translateSnapDistance / 2)
        {
            newChange.z += SnappingManager.translateSnapDistance;
            _snapLeftOvers.z -= SnappingManager.translateSnapDistance;
        }
        while (_snapLeftOvers.z < -SnappingManager.translateSnapDistance / 2)
        {
            newChange.z -= SnappingManager.translateSnapDistance;
            _snapLeftOvers.z += SnappingManager.translateSnapDistance;
        }

        return newChange;
    }

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera)
    {
        if (_gizmoRelationManager.relationSetting == GizmoRelationManager.RelationSetting.Local)
            globalChange = _entity.InverseTransformDirection(globalChange);

        switch (direction)
        {
            case TranslateDirection.XAxis:
                var changeOnX = Vector3.Project(globalChange, (_gizmoRelationManager.relationSetting == GizmoRelationManager.RelationSetting.Local) ? Vector3.right : transform.right);
                changeOnX = ApplySnapping(changeOnX);
                _entity.Translate(changeOnX, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.YAxis:
                var changeOnY = Vector3.Project(globalChange, (_gizmoRelationManager.relationSetting == GizmoRelationManager.RelationSetting.Local) ? Vector3.up : transform.up);
                changeOnY = ApplySnapping(changeOnY);
                _entity.Translate(changeOnY, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.ZAxis:
                var changeOnZ = Vector3.Project(globalChange, (_gizmoRelationManager.relationSetting == GizmoRelationManager.RelationSetting.Local) ? Vector3.forward : transform.forward);
                changeOnZ = ApplySnapping(changeOnZ);
                _entity.Translate(changeOnZ, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.XYPlane:
                globalChange = ApplySnapping(globalChange);
                _entity.Translate(globalChange.x, globalChange.y, 0, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.YZPlane:
                globalChange = ApplySnapping(globalChange);
                _entity.Translate(0, globalChange.y, globalChange.z, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.ZXPlane:
                globalChange = ApplySnapping(globalChange);
                _entity.Translate(globalChange.x, 0, globalChange.z, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.All:
                globalChange = ApplySnapping(globalChange);
                _entity.Translate(globalChange.x, globalChange.y, globalChange.z, _gizmoRelationManager.relationSetting.ToSpace());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //Debug.Log(_snapLeftOvers);
    }

    public override Plane GetPlane(Camera camera)
    {
        _snapLeftOvers = Vector3.zero;
        if (SnappingManager.IsSnapping && _gizmoRelationManager.relationSetting == GizmoRelationManager.RelationSetting.Global)
        {
            var snappedPosition = ApplySnapping(_entity.transform.position); // Sets _snapLeftOvers to an initial state to snap onto the Global grid

            // move _entity onto the global grid
            _entity.transform.position = direction switch
            {
                TranslateDirection.XAxis => new Vector3(
                    snappedPosition.x,
                    _entity.transform.position.y,
                    _entity.transform.position.z),
                TranslateDirection.YAxis => new Vector3(
                    _entity.transform.position.x,
                    snappedPosition.y,
                    _entity.transform.position.z),
                TranslateDirection.ZAxis => new Vector3(
                    _entity.transform.position.x,
                    _entity.transform.position.y,
                    snappedPosition.z),
                TranslateDirection.XYPlane => new Vector3(
                    snappedPosition.x,
                    snappedPosition.y,
                    _entity.transform.position.z),
                TranslateDirection.YZPlane => new Vector3(
                    _entity.transform.position.x,
                    snappedPosition.y,
                    snappedPosition.z),
                TranslateDirection.ZXPlane => new Vector3(
                    snappedPosition.x,
                    _entity.transform.position.y,
                    snappedPosition.z),
                TranslateDirection.All => new Vector3(
                    snappedPosition.x,
                    snappedPosition.y,
                    snappedPosition.z),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        switch (direction)
        {
            case TranslateDirection.XAxis:
                return new Plane(Vector3.Project(camera.transform.position - _entity.position, transform.right) + _entity.position - camera.transform.position, _entity.position);
            //return new Plane(Vector3.Cross(camera.transform.up,_entity.right) , _entity.position);
            case TranslateDirection.YAxis:
                return new Plane(Vector3.Project(camera.transform.position - _entity.position, transform.up) + _entity.position - camera.transform.position, _entity.position);
            //return new Plane(Vector3.Cross(camera.transform.right,_entity.up) , _entity.position);
            case TranslateDirection.ZAxis:
                return new Plane(Vector3.Project(camera.transform.position - _entity.position, transform.forward) + _entity.position - camera.transform.position, _entity.position);
            //return new Plane(Vector3.Cross(camera.transform.up,_entity.forward) , _entity.position);
            case TranslateDirection.XYPlane:
                return new Plane(transform.right, _entity.position);
            case TranslateDirection.YZPlane:
                return new Plane(transform.right, _entity.position);
            case TranslateDirection.ZXPlane:
                return new Plane(transform.right, _entity.position);
            case TranslateDirection.All:
                return new Plane(camera.transform.forward, _entity.position);
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public override Ray GetOneRay()
    {
        switch (direction)
        {
            case TranslateDirection.XYPlane:
            case TranslateDirection.XAxis:
                return new Ray(_entity.position, _entity.right);
            case TranslateDirection.YAxis:
            case TranslateDirection.YZPlane:
                return new Ray(_entity.position, _entity.up);
            case TranslateDirection.ZAxis:
            case TranslateDirection.ZXPlane:
                return new Ray(_entity.position, _entity.forward);
            case TranslateDirection.All:
                return new Ray(_entity.position, Vector3.up);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
