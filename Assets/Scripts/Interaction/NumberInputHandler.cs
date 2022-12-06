using System;
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
        TextInputHandler.SetCurrentText(NumberToString(currentContents));
    }

    public float GetCurrentNumber()
    {
        return StringToNumber(TextInputHandler.GetCurrentText());
    }

    public void ResetActions()
    {
        TextInputHandler.ResetActions();
    }

    public void SetActions(Action<float> onChange, Action<float> onSubmit, Action<float> onAbort)
    {
        TextInputHandler.SetActions(
                (value) => onChange(StringToNumber(value)),
                (value) => onSubmit(StringToNumber(value)),
                (value) => onAbort(StringToNumber(value))
            );
    }

    private string NumberToString(float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private float StringToNumber(string value)
    {
        if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        // todo return error
        return 0;
    }
}
