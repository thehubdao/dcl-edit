using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatorParent : VisualRepresentation
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
    }

    public override void UpdateVisuals()
    {
        var currentManipulator = GizmoToolManager.CurrentTool;

        if (_translate)
            _translate.SetActive(currentManipulator == GizmoToolManager.Tool.Translate);
        if (_rotation)
            _rotation.SetActive(currentManipulator == GizmoToolManager.Tool.Rotate);
        if (_scale)
            _scale.SetActive(currentManipulator == GizmoToolManager.Tool.Scale);
    }
}
