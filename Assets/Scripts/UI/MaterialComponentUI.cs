using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class MaterialComponentUI : ComponentUI
{
    [SerializeField] private TMP_InputField _albedoColorRInput;
    [SerializeField] private TMP_InputField _albedoColorGInput;
    [SerializeField] private TMP_InputField _albedoColorBInput;

    [SerializeField] private TMP_InputField _emissiveColorRInput;
    [SerializeField] private TMP_InputField _emissiveColorGInput;
    [SerializeField] private TMP_InputField _emissiveColorBInput;


    void Start()
    {
        DclSceneManager.OnSelectedEntityTransformChange.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    public override void UpdateVisuals()
    {
        var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
        if (materialComponent == null)
            return;

        var numberFormat = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();

        // Albedo Color
        var albedo = materialComponent.materialValues.albedoColor;
        _albedoColorRInput.text = albedo.r.ToString("0.###", numberFormat);
        _albedoColorGInput.text = albedo.g.ToString("0.###", numberFormat);
        _albedoColorBInput.text = albedo.b.ToString("0.###", numberFormat);


        // Emissive Color
        var emissive = materialComponent.materialValues.emissiveColor;
        _emissiveColorRInput.text = emissive.r.ToString("0.###", numberFormat);
        _emissiveColorGInput.text = emissive.g.ToString("0.###", numberFormat);
        _emissiveColorBInput.text = emissive.b.ToString("0.###", numberFormat);
    }

    private MaterialComponent.MaterialValues _valuesBeforeEditing;

    public void StartUndoRecording()
    {
        var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
        if (materialComponent == null)
            return;

        _valuesBeforeEditing = materialComponent.materialValues.Copy();

    }

    public void ApplyUndoRecording()
    {
        var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
        if (materialComponent == null)
            return;

        if (_valuesBeforeEditing != null)
        {
            var beforeValues = _valuesBeforeEditing;
            var afterValues = materialComponent.materialValues.Copy();
            
            if (!beforeValues.Equals(afterValues))
            {
                
                UndoManager.RecordUndoItem(
                    "Changed Material",
                    () =>
                    {
                        materialComponent.materialValues = beforeValues;
                        DclSceneManager.OnUpdateSelection.Invoke();
                    },
                    () =>
                    {
                        materialComponent.materialValues = afterValues;
                        DclSceneManager.OnUpdateSelection.Invoke();
                    }
                );
            }


        }

        DclSceneManager.OnUpdateSelection.Invoke();
    }

    public void SetValueAlbedoColor()
    {
        var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
        if (materialComponent == null)
            return;
        try
        {
            materialComponent.materialValues.albedoColor = new Color(
                float.Parse(_albedoColorRInput.text, CultureInfo.InvariantCulture),
                float.Parse(_albedoColorGInput.text, CultureInfo.InvariantCulture),
                float.Parse(_albedoColorBInput.text, CultureInfo.InvariantCulture));

        }
        catch
        {
            // ignored
        }
    }
    public void SetValueEmissiveColor()
    {
        var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
        if (materialComponent == null)
            return;
        try
        {
            materialComponent.materialValues.emissiveColor = new Color(
                float.Parse(_emissiveColorRInput.text, CultureInfo.InvariantCulture),
                float.Parse(_emissiveColorGInput.text, CultureInfo.InvariantCulture),
                float.Parse(_emissiveColorBInput.text, CultureInfo.InvariantCulture));

        }
        catch
        {
            // ignored
        }
    }
}
