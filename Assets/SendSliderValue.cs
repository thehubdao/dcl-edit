using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendSliderValue : MonoBehaviour
{
    [SerializeField]
    private GizmoSizeManager _gizmoSizeManager;

    [SerializeField]
    private Slider _slider;

    // Update is called once per frame
    void Update()
    {
        _gizmoSizeManager.SetSize(_slider.value);
    }
}
