using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class TransformComponentUI : ComponentUI
{
    [SerializeField] private TMP_InputField _translateXInput = default;
    [SerializeField] private TMP_InputField _translateYInput = default;
    [SerializeField] private TMP_InputField _translateZInput = default;

    [SerializeField] private TMP_InputField _rotateXInput = default;
    [SerializeField] private TMP_InputField _rotateYInput = default;
    [SerializeField] private TMP_InputField _rotateZInput = default;

    [SerializeField] private TMP_InputField _scaleXInput = default;
    [SerializeField] private TMP_InputField _scaleYInput = default;
    [SerializeField] private TMP_InputField _scaleZInput = default;

    void Start()
    {
        DclSceneManager.OnSelectedEntityTransformChange.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    public override void UpdateVisuals()
    {
        var transformComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<TransformComponent>();
        if (transformComponent == null)
            return;

        var numberFormat = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();

        // Position
        var pos = transformComponent.transform.localPosition;
        _translateXInput.text = pos.x.ToString("0.###", numberFormat);
        _translateYInput.text = pos.y.ToString("0.###", numberFormat);
        _translateZInput.text = pos.z.ToString("0.###", numberFormat);

        // Rotation
        var eulerRot = transformComponent.transform.localEulerAngles;
        _rotateXInput.text = eulerRot.x.ToString("0.#", numberFormat);
        _rotateYInput.text = eulerRot.y.ToString("0.#", numberFormat);
        _rotateZInput.text = eulerRot.z.ToString("0.#", numberFormat);

        // Scale
        var scale = transformComponent.entity.transform.localScale;
        _scaleXInput.text = scale.x.ToString("0.###", numberFormat);
        _scaleYInput.text = scale.y.ToString("0.###", numberFormat);
        _scaleZInput.text = scale.z.ToString("0.###", numberFormat);

    }

    private TransformUndo _transformUndo = null;

    public void StartUndoRecording()
    {
        _transformUndo = new TransformUndo(DclSceneManager.PrimarySelectedEntity.AsSingleInstanceInEnumerable());
        _transformUndo.SaveBeginningState();

        Debug.Log("start recording");
    }

    public void ApplyUndoRecording()
    {
        if (_transformUndo != null)
        {
            _transformUndo.SaveEndingState();
            _transformUndo.AddUndoItem();
            _transformUndo = null;
            Debug.Log("recorded");

        }
    }

    public void SetValueTranslate()
    {
        var transformComponent = DclSceneManager.PrimarySelectedEntity.GetComponent<TransformComponent>();
        try
        {
            var newLocalPosition = new Vector3(
                float.Parse(_translateXInput.text, CultureInfo.InvariantCulture),
                float.Parse(_translateYInput.text, CultureInfo.InvariantCulture),
                float.Parse(_translateZInput.text, CultureInfo.InvariantCulture));

            if (newLocalPosition != transformComponent.transform.localPosition)
            {
                transformComponent.transform.localPosition = newLocalPosition;
                DclSceneManager.OnSelectedEntityTransformChange.Invoke();
            }
        }
        catch
        {
            // ignored
        }
    }
    public void SetValueRotate()
    {
        var transformComponent = DclSceneManager.PrimarySelectedEntity.GetComponent<TransformComponent>();
        try
        {
            var newLocalEulerAngles = new Vector3(
                float.Parse(_rotateXInput.text, CultureInfo.InvariantCulture),
                float.Parse(_rotateYInput.text, CultureInfo.InvariantCulture),
                float.Parse(_rotateZInput.text, CultureInfo.InvariantCulture));

            if (transformComponent.transform.localEulerAngles != newLocalEulerAngles)
            {
                transformComponent.transform.localEulerAngles = newLocalEulerAngles;
                DclSceneManager.OnSelectedEntityTransformChange.Invoke();
            }
        }
        catch
        {
            // ignored
        }
    }
    public void SetValueScale()
    {
        var transformComponent = DclSceneManager.PrimarySelectedEntity.GetComponent<TransformComponent>();
        try
        {
            var newLocalScale = new Vector3(
                float.Parse(_scaleXInput.text, CultureInfo.InvariantCulture),
                float.Parse(_scaleYInput.text, CultureInfo.InvariantCulture),
                float.Parse(_scaleZInput.text, CultureInfo.InvariantCulture));

            if (transformComponent.transform.localScale != newLocalScale)
            {
                transformComponent.transform.localScale = newLocalScale;
                DclSceneManager.OnSelectedEntityTransformChange.Invoke();
            }
        }
        catch
        {
            // ignored
        }
    }

}
