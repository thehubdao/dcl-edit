using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolVisuals : VisualRepresentation
{
    [SerializeField]
    private GameObject _translate;
    [SerializeField]
    private GameObject _rotation;
    [SerializeField]
    private GameObject _scale;

    

    void Start()
    {
        GizmoToolManager.onUpdate.AddListener(SetDirty);
        GizmoRelationManager.onUpdate.AddListener(SetDirty);
        DclSceneManager.OnUpdateSelection.AddListener(SetDirty);
        DclSceneManager.OnSelectedEntityTransformChange.AddListener(SetDirty);
    }

    public override void UpdateVisuals()
    {
        if (DclSceneManager.PrimarySelectedEntity == null)
        {
            _translate.SetActive(false);
            _rotation.SetActive(false);
            _scale.SetActive(false);
            return;
        }


        // show the correct tool
        var currentManipulator = GizmoToolManager.CurrentTool;

        if (_translate)
            _translate.SetActive(currentManipulator == GizmoToolManager.Tool.Translate);
        if (_rotation)
            _rotation.SetActive(currentManipulator == GizmoToolManager.Tool.Rotate);
        if (_scale)
            _scale.SetActive(currentManipulator == GizmoToolManager.Tool.Scale);

        // move the tools to the correct position
        transform.position = DclSceneManager.PrimarySelectedEntity.transform.position;

        // Rotate the tools into the correct orientation
        if (DclSceneManager.PrimarySelectedEntity != null)
        {
            if (GizmoToolManager.CurrentTool == GizmoToolManager.Tool.Scale)
            {
                transform.localRotation = DclSceneManager.PrimarySelectedEntity.transform.rotation;
                return;
            }

            switch (GizmoRelationManager.RelationSetting)
            {
                case GizmoRelationManager.RelationSettingEnum.Local:
                    transform.rotation = DclSceneManager.PrimarySelectedEntity.transform.rotation;
                    break;
                case GizmoRelationManager.RelationSettingEnum.Global:
                    transform.rotation = Quaternion.identity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
