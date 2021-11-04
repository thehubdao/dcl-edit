using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnappingButton : MonoBehaviour
{
    void Start()
    {
        SnappingManager.onSnappingSettingsChange.AddListener(UpdateVisuals);
    }

    [SerializeField]
    private TextMeshProUGUI _text;

    private void UpdateVisuals()
    {
        _text.text = SnappingManager.IsSnapping ? "Snap\nOn" : "Snap\nOff";
    }

    public void SwitchSnapping()
    {
        SnappingManager.IsSnapping = !SnappingManager.IsSnapping;
    }
}
