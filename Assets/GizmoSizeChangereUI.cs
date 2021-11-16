using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GizmoSizeChangereUI : MonoBehaviour
{

    [SerializeField]
    private Slider _slider;


    void OnEnable()
    {
        UpdateVisuals();
        _slider.onValueChanged.AddListener(SetSize);
    }

    private void UpdateVisuals()
    {
        _slider.value = SceneManager.GizmoScale;
    }

    public void SetSize(Single value)
    {
        SceneManager.GizmoScale = value;
        Debug.Log(value);
        
    }    
    
}
