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
        GizmoManipulatorManager.onUpdate.AddListener(SetDirty);
    }

    public override void UpdateVisuals()
    {
        var currentManipulator = GizmoManipulatorManager.CurrentManipulator;

        if (_translate)
            _translate.SetActive(currentManipulator == GizmoManipulatorManager.Manipulator.Translate);
        if (_rotation)
            _rotation.SetActive(currentManipulator == GizmoManipulatorManager.Manipulator.Rotate);
        if (_scale)
            _scale.SetActive(currentManipulator == GizmoManipulatorManager.Manipulator.Scale);
    }
}
