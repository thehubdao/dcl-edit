using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class NumberInputHandler : MonoBehaviour
{
    [SerializeField]
    public TextInputHandler TextInputHandler;


    public void SetCurrentNumber(float currentContents)
    {
        TextInputHandler.SetCurrentText(currentContents.ToString(CultureInfo.InvariantCulture));
    }
}
