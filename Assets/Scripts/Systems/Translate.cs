using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //private Transform _entity;
    void Start()
    {
        //_entity = GetComponentInParent<Entity>().transform;
    }

    private Vector3 _snapLeftOvers;

    private Vector3 ApplySnapping(Vector3 change)
    {
        var snapping =
            SnappingManager.IsSnapping ^ 
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));

        if (!snapping)
        {
            return change;
        }

        _snapLeftOvers += change;
        var newChange = Vector3.zero;

        while (_snapLeftOvers.x > SnappingManager.TranslateSnapDistance / 2)
        {
            newChange.x += SnappingManager.TranslateSnapDistance;
            _snapLeftOvers.x -= SnappingManager.TranslateSnapDistance;
        }
        while (_snapLeftOvers.x < -SnappingManager.TranslateSnapDistance / 2)
        {
            newChange.x -= SnappingManager.TranslateSnapDistance;
            _snapLeftOvers.x += SnappingManager.TranslateSnapDistance;
        }

        while (_snapLeftOvers.y > SnappingManager.TranslateSnapDistance / 2)
        {
            newChange.y += SnappingManager.TranslateSnapDistance;
            _snapLeftOvers.y -= SnappingManager.TranslateSnapDistance;
        }
        while (_snapLeftOvers.y < -SnappingManager.TranslateSnapDistance / 2)
        {
            newChange.y -= SnappingManager.TranslateSnapDistance;
            _snapLeftOvers.y += SnappingManager.TranslateSnapDistance;
        }

        while (_snapLeftOvers.z > SnappingManager.TranslateSnapDistance / 2)
        {
            newChange.z += SnappingManager.TranslateSnapDistance;
            _snapLeftOvers.z -= SnappingManager.TranslateSnapDistance;
        }
        while (_snapLeftOvers.z < -SnappingManager.TranslateSnapDistance / 2)
        {
            newChange.z -= SnappingManager.TranslateSnapDistance;
            _snapLeftOvers.z += SnappingManager.TranslateSnapDistance;
        }

        return newChange;
    }

    public override void Change(Vector3 globalChange, Vector3 localChange, Vector3 cameraSpaceChange, Camera gizmoCamera)
    {
        var entities = SceneManager.AllSelectedEntitiesWithoutChildren.Select(entity => entity.transform);
        var primaryEntity = SceneManager.PrimarySelectedEntity;

        //foreach (var entity in SceneManager.AllSelectedEntities.Select(entity => entity.transform))
        //{
            if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                globalChange = primaryEntity.transform.InverseTransformDirection(globalChange);

            switch (direction)
            {
                case TranslateDirection.XAxis:
                    var changeOnX = Vector3.Project(globalChange, (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local) ? Vector3.right : transform.right);
                    changeOnX = ApplySnapping(changeOnX);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        changeOnX = primaryEntity.transform.TransformDirection(changeOnX);
                    entities.Forall(entity => entity.Translate(changeOnX,Space.World));
                    break;
                case TranslateDirection.YAxis:
                    var changeOnY = Vector3.Project(globalChange, (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local) ? Vector3.up : transform.up);
                    changeOnY = ApplySnapping(changeOnY);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        changeOnY = primaryEntity.transform.TransformDirection(changeOnY);
                    entities.Forall(entity => entity.Translate(changeOnY, Space.World));
                    break;
                case TranslateDirection.ZAxis:
                    var changeOnZ = Vector3.Project(globalChange, (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local) ? Vector3.forward : transform.forward);
                    changeOnZ = ApplySnapping(changeOnZ);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        changeOnZ = primaryEntity.transform.TransformDirection(changeOnZ);
                    entities.Forall(entity => entity.Translate(changeOnZ, Space.World));
                    break;
                case TranslateDirection.XYPlane:
                    globalChange = ApplySnapping(globalChange);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        globalChange = primaryEntity.transform.TransformDirection(new Vector3(globalChange.x,globalChange.y,0));
                    entities.Forall(entity => entity.Translate(globalChange, Space.World));
                    break;
                case TranslateDirection.YZPlane:
                    globalChange = ApplySnapping(globalChange);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        globalChange = primaryEntity.transform.TransformDirection(new Vector3(0, globalChange.y, globalChange.z));
                    entities.Forall(entity => entity.Translate(globalChange, Space.World));
                    break;
                case TranslateDirection.ZXPlane:
                    globalChange = ApplySnapping(globalChange);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        globalChange = primaryEntity.transform.TransformDirection(new Vector3(globalChange.x, 0, globalChange.z));
                    entities.Forall(entity => entity.Translate(globalChange, Space.World));
                    break;
                case TranslateDirection.All:
                    globalChange = ApplySnapping(globalChange);
                    if (GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Local)
                        globalChange = primaryEntity.transform.TransformDirection(globalChange);
                    entities.Forall(entity => entity.Translate(globalChange, Space.World));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Debug.Log(_snapLeftOvers);
        //}
    }

    public override Plane GetPlane(Camera camera)
    {

        var entity = SceneManager.PrimarySelectedEntity.transform;

        _snapLeftOvers = Vector3.zero;
        if (SnappingManager.IsSnapping && GizmoRelationManager.RelationSetting == GizmoRelationManager.RelationSettingEnum.Global)
        {
            var snappedPosition = ApplySnapping(entity.transform.position); // Sets _snapLeftOvers to an initial state to snap onto the Global grid

            // move _entity onto the global grid
            entity.transform.position = direction switch
            {
                TranslateDirection.XAxis => new Vector3(
                    snappedPosition.x,
                    entity.transform.position.y,
                    entity.transform.position.z),
                TranslateDirection.YAxis => new Vector3(
                    entity.transform.position.x,
                    snappedPosition.y,
                    entity.transform.position.z),
                TranslateDirection.ZAxis => new Vector3(
                    entity.transform.position.x,
                    entity.transform.position.y,
                    snappedPosition.z),
                TranslateDirection.XYPlane => new Vector3(
                    snappedPosition.x,
                    snappedPosition.y,
                    entity.transform.position.z),
                TranslateDirection.YZPlane => new Vector3(
                    entity.transform.position.x,
                    snappedPosition.y,
                    snappedPosition.z),
                TranslateDirection.ZXPlane => new Vector3(
                    snappedPosition.x,
                    entity.transform.position.y,
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
                return new Plane(Vector3.Project(camera.transform.position - entity.position, transform.right) + entity.position - camera.transform.position, entity.position);
            //return new Plane(Vector3.Cross(camera.transform.up,_entity.right) , _entity.position);
            case TranslateDirection.YAxis:
                return new Plane(Vector3.Project(camera.transform.position - entity.position, transform.up) + entity.position - camera.transform.position, entity.position);
            //return new Plane(Vector3.Cross(camera.transform.right,_entity.up) , _entity.position);
            case TranslateDirection.ZAxis:
                return new Plane(Vector3.Project(camera.transform.position - entity.position, transform.forward) + entity.position - camera.transform.position, entity.position);
            //return new Plane(Vector3.Cross(camera.transform.up,_entity.forward) , _entity.position);
            case TranslateDirection.XYPlane:
                return new Plane(transform.right, entity.position);
            case TranslateDirection.YZPlane:
                return new Plane(transform.right, entity.position);
            case TranslateDirection.ZXPlane:
                return new Plane(transform.right, entity.position);
            case TranslateDirection.All:
                return new Plane(camera.transform.forward, entity.position);
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public override Ray GetOneRay()
    {
        var entity = SceneManager.PrimarySelectedEntity.transform;

        switch (direction)
        {
            case TranslateDirection.XYPlane:
            case TranslateDirection.XAxis:
                return new Ray(entity.position, entity.right);
            case TranslateDirection.YAxis:
            case TranslateDirection.YZPlane:
                return new Ray(entity.position, entity.up);
            case TranslateDirection.ZAxis:
            case TranslateDirection.ZXPlane:
                return new Ray(entity.position, entity.forward);
            case TranslateDirection.All:
                return new Ray(entity.position, Vector3.up);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
