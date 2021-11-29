using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateManipulators : VisualRepresentation
{
    

    void Start()
    {
        GizmoRelationManager.onUpdate.AddListener(SetDirty);
        GizmoToolManager.onUpdate.AddListener(SetDirty);
    }

    public override void UpdateVisuals()
    {
        if (SceneManager.PrimarySelectedEntity != null)
        {
            if (GizmoToolManager.CurrentTool == GizmoToolManager.Tool.Scale)
            {
                SceneManager.PrimarySelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
                return;
            }

            switch (GizmoRelationManager.RelationSetting)
            {
                case GizmoRelationManager.RelationSettingEnum.Local:
                    SceneManager.PrimarySelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
                    break;
                case GizmoRelationManager.RelationSettingEnum.Global:
                    SceneManager.PrimarySelectedEntity.gizmos.transform.rotation = Quaternion.identity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
