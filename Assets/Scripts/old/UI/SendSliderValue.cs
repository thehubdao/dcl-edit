using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendSliderValue : MonoBehaviour
{

    [SerializeField]
    private Slider _slider;

    // Update is called once per frame
    void Update()
    {
        GizmoSizeManager.GizmoScale = _slider.value;
    }
}
