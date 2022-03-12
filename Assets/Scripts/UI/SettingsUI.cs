using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField]
    private TMP_InputField _cameraSpeedText;
    [SerializeField]
    private TMP_InputField _mouseSensitivityText;

    [Space(5)]
    [SerializeField]
    private TMP_InputField _translateSnappingText;
    [SerializeField]
    private TMP_InputField _rotateSnappingText;
    [SerializeField]
    private TMP_InputField _scaleSnappingText;


    [Header("Editor")]
    [SerializeField]
    private TMP_InputField _uiScaleText;

    [SerializeField]
    private TMP_InputField _frameRateCap;
    [SerializeField]
    private Toggle _framerateCapToggle;
    [Header("Generator")]
    [SerializeField]
    private TMP_InputField _scriptLocationText;


    private bool _isDirty = false;

    private void AddEndEditListener(ref TMP_InputField inputField, Action<string> handler)
    {
        inputField.onEndEdit.AddListener(value =>
        {
            try
            {
                handler(value);
            }
            catch (Exception)
            {
                // ignored
            }
            UpdateVisuals();
        });
    }

    void Awake()
    {
        AddEndEditListener(ref _cameraSpeedText, value => PersistentData.CameraSpeed = float.Parse(value));
        AddEndEditListener(ref _mouseSensitivityText, value => PersistentData.MouseSensitivity = float.Parse(value));

        AddEndEditListener(ref _translateSnappingText, value => ProjectData.translateSnapStep = float.Parse(value));
        AddEndEditListener(ref _rotateSnappingText, value => ProjectData.rotateSnapStep = float.Parse(value));
        AddEndEditListener(ref _scaleSnappingText, value => ProjectData.scaleSnapStep = float.Parse(value));

        AddEndEditListener(ref _uiScaleText, value => CanvasManager.UiScale = float.Parse(value));
        AddEndEditListener(ref _frameRateCap, value => PersistentData.FramerateCap = int.Parse(value));
        AddEndEditListener(ref _scriptLocationText, value => ProjectData.generateScriptLocation = value);
        _framerateCapToggle.onValueChanged.AddListener(delegate { FrmerateCapToggleChanged(_framerateCapToggle); });

        CanvasManager.onUiChange.AddListener(SetDirty);
    }

    private void FrmerateCapToggleChanged(Toggle framerateCapToggle)
    {
        if (framerateCapToggle.isOn)
        {
            PersistentData.FramerateCap = Screen.currentResolution.refreshRate;
        }
        else
        {
            PersistentData.FramerateCap = 0;
        }
    }

    public void SetDirty()
    {
        _isDirty = true;
    }

    void LateUpdate()
    {
        if (_isDirty)
        {
            _isDirty = false;
            UpdateVisuals();
        }
    }

    void OnEnable()
    {
        SetDirty();
    }

    private void UpdateVisuals()
    {
        //Debug.Log("update settings visuals");

        _cameraSpeedText.text = PersistentData.CameraSpeed.ToString("#0.###");
        _mouseSensitivityText.text = PersistentData.MouseSensitivity.ToString("#0.###");

        _translateSnappingText.text = ProjectData.translateSnapStep.ToString("#0.###");
        _rotateSnappingText.text = ProjectData.rotateSnapStep.ToString("#0.###");
        _scaleSnappingText.text = ProjectData.scaleSnapStep.ToString("#0.###");

        _uiScaleText.text = CanvasManager.UiScale.ToString("#0.###");
        _frameRateCap.text = PersistentData.FramerateCap.ToString();
        if (PersistentData.FramerateCap > 0)
        {
            _framerateCapToggle.isOn=true;
            _frameRateCap.ActivateInputField();
        }
        else
        {
            _framerateCapToggle.isOn = false;
            _frameRateCap.DeactivateInputField();
        }
        _scriptLocationText.text = ProjectData.generateScriptLocation;

    }
}
