using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateManipulators : VisualRepresentation
{

    private GizmoRelationManager relationManager;
    private GizmoManipulatorManager manipulatorManager;

    void Start()
    {
        relationManager = GetComponentInParent<GizmoRelationManager>();
        relationManager.OnUpdate.AddListener(SetDirty);
        manipulatorManager = GetComponentInParent<GizmoManipulatorManager>();
        manipulatorManager.OnUpdate.AddListener(SetDirty);
    }

    public override void UpdateVisuals()
    {
        if (SceneManager.SelectedEntity != null)
        {
            if (manipulatorManager.CurrentManipulator == GizmoManipulatorManager.Manipulator.Scale)
            {
                SceneManager.SelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
                return;
            }

            switch (relationManager.relationSetting)
            {
                case GizmoRelationManager.RelationSetting.Local:
                    SceneManager.SelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
                    break;
                case GizmoRelationManager.RelationSetting.Global:
                    SceneManager.SelectedEntity.gizmos.transform.rotation = Quaternion.identity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
