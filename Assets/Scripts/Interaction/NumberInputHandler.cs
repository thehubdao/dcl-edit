using System;
using System.Globalization;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

public class NumberInputHandler : MonoBehaviour
{
    // Dependencies
    [SerializeField]
    public TextInputHandler TextInputHandler;
    private NumberInputSystem _numberInputSystem;

    [Inject]
    private void Construct(NumberInputSystem numberInputSystem)
    {
        _numberInputSystem = numberInputSystem;
    }
    public void SetCurrentNumber(float currentContents)
    {
        TextInputHandler.SetCurrentText(NumberToString(currentContents));
    }

    public float? GetCurrentNumber()
    {
        return _numberInputSystem.ValidateNumberInput(TextInputHandler.GetCurrentText());
    }

    public void ResetActions()
    {
        TextInputHandler.ResetActions();
    }

    public void SetActions(Action<float> onChange,  Action onInvalid, Action<float> onSubmit, Action<float> onAbort)
    {
        TextInputHandler.SetActions(
                (value) =>
                {
                    var numberValue = _numberInputSystem.ValidateNumberInput(value);
                    if (numberValue == null)
                    {
                        onInvalid();
                        TextInputHandler.SetBorderColorValid(false);
                    }
                    else
                    {
                        onChange(numberValue.Value);
                        TextInputHandler.SetBorderColorValid(true);
                    }
                },
                (value) =>
                {
                    var numberValue = _numberInputSystem.ValidateNumberInput(value);
                    if (numberValue == null)
                    {
                        onAbort(0);
                    }
                    else
                    {
                        onSubmit(numberValue.Value);
                    }
                },
                (value) =>
                {
                    var numberValue = _numberInputSystem.ValidateNumberInput(value);
                    if (numberValue == null)
                    {
                        onAbort(0);
                    }
                    else
                    {
                        onAbort(numberValue.Value);
                    }
                });
    }

    private string NumberToString(float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}

