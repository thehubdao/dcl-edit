using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class ManipulatorParent : MonoBehaviour
{
    [SerializeField]
    private GameObject _translate;
    [SerializeField]
    private GameObject _rotation;
    [SerializeField]
    private GameObject _scale;

    void OnEnable()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        var currentManipulator = GetComponentInParent<GizmoManipulatorManager>().CurrentManipulator;

        if (_translate)
            _translate.SetActive(currentManipulator == GizmoManipulatorManager.Manipulator.Translate);
        if (_rotation)
            _rotation.SetActive(currentManipulator == GizmoManipulatorManager.Manipulator.Rotate);
        if (_scale)
            _scale.SetActive(currentManipulator == GizmoManipulatorManager.Manipulator.Scale);
    }
}
