using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManipulatorSwitchButton : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Button _translateButton;
    [SerializeField]
    private Button _rotateButton;
    [SerializeField]
    private Button _scaleButton;

    public ManipulatorSwitchButton(TextMeshProUGUI text)
    {
        this._text = text;
    }

    void Start()
    {
        GizmoToolManager.onUpdate.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if(_text!=null)
            _text.text = GizmoToolManager.CurrentTool.ToString();

        if (_translateButton == null || _rotateButton == null || _scaleButton == null)
            return;

        _translateButton.interactable = GizmoToolManager.CurrentTool != GizmoToolManager.Tool.Translate;
        _rotateButton.interactable = GizmoToolManager.CurrentTool != GizmoToolManager.Tool.Rotate;
        _scaleButton.interactable = GizmoToolManager.CurrentTool != GizmoToolManager.Tool.Scale;
        
    }

    public void SetNextManipulator()
    {
        GizmoToolManager.SwitchToNextManipulator();
    }

    public void SetManipulator(GizmoToolManager.Tool tool) 
    {
        GizmoToolManager.CurrentTool = tool;
    }

    public void SetManipulatorTranslate()
    {
        SetManipulator(GizmoToolManager.Tool.Translate);
    }
    public void SetManipulatorRotate()
    {
        SetManipulator(GizmoToolManager.Tool.Rotate);
    }
    public void SetManipulatorScale()
    {
        SetManipulator(GizmoToolManager.Tool.Scale);
    }
}
