using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateManipulators : VisualRepresentation
{
    
    private GizmoManipulatorManager manipulatorManager;

    void Start()
    {
        GizmoRelationManager.onUpdate.AddListener(SetDirty);
        manipulatorManager = GetComponentInParent<GizmoManipulatorManager>();
        GizmoManipulatorManager.onUpdate.AddListener(SetDirty);
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

            switch (GizmoRelationManager.RelationSetting)
            {
                case GizmoRelationManager.RelationSettingEnum.Local:
                    SceneManager.SelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
                    break;
                case GizmoRelationManager.RelationSettingEnum.Global:
                    SceneManager.SelectedEntity.gizmos.transform.rotation = Quaternion.identity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
