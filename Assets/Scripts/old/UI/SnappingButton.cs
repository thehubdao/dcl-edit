using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnappingButton : MonoBehaviour
{
    void Start()
    {
        SnappingManager.onSnappingSettingsChange.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Button _snappingButton;

    [SerializeField]
    private Color _activeColor;

    [SerializeField]
    private Color _inactiveColor;

    

    private void UpdateVisuals()
    {
        if(_text!=null)
            _text.text = SnappingManager.IsSnapping ? "Snap\nOn" : "Snap\nOff";

        if(_snappingButton!=null)
        {
            var c = _snappingButton.colors;
            c.normalColor = SnappingManager.IsSnapping ? _activeColor : _inactiveColor;
            c.selectedColor = SnappingManager.IsSnapping ? _activeColor : _inactiveColor;
            _snappingButton.colors = c;
        }
    }

    public void SwitchSnapping()
    {
        SnappingManager.IsSnapping = !SnappingManager.IsSnapping;
    }
}
