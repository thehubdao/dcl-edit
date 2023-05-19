using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class MaterialComponentUI : ComponentUI
{

    [SerializeField]
    private ColorProperty _albedoColorProperty = default;
    [SerializeField]
    private ColorProperty _emissiveColorProperty = default;

    void Start()
    {
        DclSceneManager.OnSelectedEntityTransformChange.AddListener(UpdateVisuals);

        _albedoColorProperty.OnClickedColorField.AddListener(() =>
        {
            var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
            if (materialComponent == null)
                return;

            var currentAlbedoColor = materialComponent.materialValues.albedoColor;

            //OpenColorPickerWindowSystem.RequestColor(currentAlbedoColor, color =>
            //{
            //    var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
            //    if (materialComponent == null)
            //        return;

            //    var oldColor = materialComponent.materialValues.albedoColor;

            //    materialComponent.materialValues.albedoColor = color;

            //    if (!oldColor.Equals(color))
            //    {

            //        UndoManager.RecordUndoItem(
            //            "Changed Material Albedo Color",
            //            () =>
            //            {
            //                materialComponent.materialValues.albedoColor = oldColor;
            //                DclSceneManager.OnUpdateSelection.Invoke();
            //            },
            //            () =>
            //            {
            //                materialComponent.materialValues.albedoColor = color;
            //                DclSceneManager.OnUpdateSelection.Invoke();
            //            }
            //        );
            //    }
            //    DclSceneManager.OnUpdateSelection.Invoke();
            //});
        });

        _emissiveColorProperty.OnClickedColorField.AddListener(() =>
        {
            var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
            if (materialComponent == null)
                return;

            var currentEmissiveColor = materialComponent.materialValues.emissiveColor;

            //OpenColorPickerWindowSystem.RequestColor(currentEmissiveColor, color =>
            //{
            //    var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
            //    if (materialComponent == null)
            //        return;

            //    var oldColor = materialComponent.materialValues.emissiveColor;

            //    materialComponent.materialValues.emissiveColor = color;

            //    if (!oldColor.Equals(color))
            //    {

            //        UndoManager.RecordUndoItem(
            //            "Changed Material Emissive Color",
            //            () =>
            //            {
            //                materialComponent.materialValues.emissiveColor = oldColor;
            //                DclSceneManager.OnUpdateSelection.Invoke();
            //            },
            //            () =>
            //            {
            //                materialComponent.materialValues.emissiveColor = color;
            //                DclSceneManager.OnUpdateSelection.Invoke();
            //            }
            //        );
            //    }
            //    DclSceneManager.OnUpdateSelection.Invoke();
            //});
        });
        UpdateVisuals();
    }

    public override void UpdateVisuals()
    {
        var materialComponent = DclSceneManager.PrimarySelectedEntity?.GetComponent<MaterialComponent>();
        if (materialComponent == null)
            return;

        _albedoColorProperty.SetColor(materialComponent.materialValues.albedoColor);
        _emissiveColorProperty.SetColor(materialComponent.materialValues.emissiveColor);
    }
}
