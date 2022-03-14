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
        DclSceneManager.OnUpdateSelection.AddListener(UpdateVisuals);
        _toggleButton.onValueChanged.AddListener(SetExpose);
    }

    public void UpdateVisuals()
    {
        if (DclSceneManager.PrimarySelectedEntity == null)
            return;

        if (_toggleButton.isOn != DclSceneManager.PrimarySelectedEntity.WantsToBeExposed)
            _toggleButton.isOn = DclSceneManager.PrimarySelectedEntity.WantsToBeExposed;

        if (DclSceneManager.PrimarySelectedEntity.ExposeFailed)
        {
            _exposedText.color = Color.red; // TODO: use Color manager
            _exposedText.text = "Failed to expose";
        }
        else
        {
            _exposedText.color = Color.white; // TODO: use Color manager
            _exposedText.text = DclSceneManager.PrimarySelectedEntity.Exposed ? DclSceneManager.PrimarySelectedEntity.ExposedSymbol : "";
        }
    }

    public void SetExpose(bool _)
    {
        if (DclSceneManager.PrimarySelectedEntity == null)
            return;
        
        if(DclSceneManager.PrimarySelectedEntity.Exposed == _toggleButton.isOn)
            return;

        DclSceneManager.PrimarySelectedEntity.Exposed = _toggleButton.isOn;

        // Undo stuff
        
        var selectedEnt = DclSceneManager.PrimarySelectedEntity;
        var exposedState = _toggleButton.isOn;

        UndoManager.RecordUndoItem($"{(exposedState ? "Exposed" : "Unexposed")} {selectedEnt}",
            () =>
            {
                selectedEnt.Exposed = !exposedState;
            },
            () =>
            {
                selectedEnt.Exposed = exposedState;
            });
    }
}
