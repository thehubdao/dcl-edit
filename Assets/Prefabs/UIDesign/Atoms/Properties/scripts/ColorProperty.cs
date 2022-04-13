using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorProperty : MonoBehaviour
{
    [SerializeField]
    private Image colorFill;

    [SerializeField]
    private TMP_InputField colorText;

    public UnityEvent OnClickedColorField = new UnityEvent();

    public void ClickedColorField()
    {
        OnClickedColorField.Invoke();
    }

    public void SetColor(Color color)
    {
        colorFill.color = color;
        colorText.text = "#" + ColorUtility.ToHtmlStringRGB(color);
    }
}
