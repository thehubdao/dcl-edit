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
        if (SceneManager.SelectedEntity == null)
            return;

        if(_toggleButton.isOn != SceneManager.SelectedEntity.WantsToBeExposed)
            _toggleButton.isOn = SceneManager.SelectedEntity.WantsToBeExposed;

        if (SceneManager.SelectedEntity.ExposeFailed)
        {
            _exposedText.color = Color.red; // TODO: use Color manager
            _exposedText.text = "Failed to expose";
        }
        else
        {
            _exposedText.color = Color.white; // TODO: use Color manager
            _exposedText.text = SceneManager.SelectedEntity.Exposed ? SceneManager.SelectedEntity.ExposedSymbol : "";
        }
    }

    public void SetExpose(bool _)
    {
        if (SceneManager.SelectedEntity == null)
            return;
        
        SceneManager.SelectedEntity.Exposed = _toggleButton.isOn;
    }
}
