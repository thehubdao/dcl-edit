using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExposeToScriptUI : MonoBehaviour
{
    [SerializeField]
    private Toggle _toggleButton = default;

    [SerializeField]
    private TextMeshProUGUI _exposedText = default;

    void Start()
    {
        UpdateVisuals();
        SceneManager.OnUpdateSelection.AddListener(UpdateVisuals);
        _toggleButton.onValueChanged.AddListener(SetExpose);
    }

    public void UpdateVisuals()
    {
        if (SceneManager.PrimarySelectedEntity == null)
            return;

        if(_toggleButton.isOn != SceneManager.PrimarySelectedEntity.WantsToBeExposed)
            _toggleButton.isOn = SceneManager.PrimarySelectedEntity.WantsToBeExposed;

        if (SceneManager.PrimarySelectedEntity.ExposeFailed)
        {
            _exposedText.color = Color.red; // TODO: use Color manager
            _exposedText.text = "Failed to expose";
        }
        else
        {
            _exposedText.color = Color.white; // TODO: use Color manager
            _exposedText.text = SceneManager.PrimarySelectedEntity.Exposed ? SceneManager.PrimarySelectedEntity.ExposedSymbol : "";
        }
    }

    public void SetExpose(bool _)
    {
        if (SceneManager.PrimarySelectedEntity == null)
            return;
        
        SceneManager.PrimarySelectedEntity.Exposed = _toggleButton.isOn;
    }
}
