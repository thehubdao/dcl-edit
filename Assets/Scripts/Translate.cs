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

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange)
    {
        if(_gizmoRelationManager.relationSetting == GizmoRelationManager.RelationSetting.Local)
            globalChange = _entity.InverseTransformDirection(globalChange);
        
        switch (direction)
        {
            case TranslateDirection.XAxis:
                var changeOnX = Vector3.Project(globalChange, (_gizmoRelationManager.relationSetting==GizmoRelationManager.RelationSetting.Local)?Vector3.right: transform.right);
                _entity.Translate(changeOnX,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.YAxis:
                var changeOnY = Vector3.Project(globalChange, (_gizmoRelationManager.relationSetting==GizmoRelationManager.RelationSetting.Local)?Vector3.up: transform.up);
                _entity.Translate( changeOnY,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.ZAxis:
                var changeOnZ = Vector3.Project(globalChange, (_gizmoRelationManager.relationSetting==GizmoRelationManager.RelationSetting.Local)?Vector3.forward: transform.forward);
                _entity.Translate( changeOnZ,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.XYPlane:
                _entity.Translate( globalChange.x, globalChange.y, 0,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.YZPlane:
                _entity.Translate( 0, globalChange.y, globalChange.z,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.ZXPlane:
                _entity.Translate( globalChange.x, 0, globalChange.z,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            case TranslateDirection.All:
                _entity.Translate( globalChange.x, globalChange.y, globalChange.z,_gizmoRelationManager.relationSetting.ToSpace());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override Plane GetPlane(Camera camera)
    {
        switch (direction)
        {
            case TranslateDirection.XAxis:
                return new Plane(Vector3.Project(camera.transform.position-_entity.position,transform.right)+_entity.position-camera.transform.position , _entity.position);
                //return new Plane(Vector3.Cross(camera.transform.up,_entity.right) , _entity.position);
            case TranslateDirection.YAxis:
                return new Plane(Vector3.Project(camera.transform.position-_entity.position,transform.up)+_entity.position-camera.transform.position , _entity.position);
                //return new Plane(Vector3.Cross(camera.transform.right,_entity.up) , _entity.position);
            case TranslateDirection.ZAxis:
                return new Plane(Vector3.Project(camera.transform.position-_entity.position,transform.forward)+_entity.position-camera.transform.position , _entity.position);
                //return new Plane(Vector3.Cross(camera.transform.up,_entity.forward) , _entity.position);
            case TranslateDirection.XYPlane:
                return new Plane(transform.right , _entity.position);
            case TranslateDirection.YZPlane:
                return new Plane(transform.right , _entity.position);
            case TranslateDirection.ZXPlane:
                return new Plane(transform.right , _entity.position);
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
