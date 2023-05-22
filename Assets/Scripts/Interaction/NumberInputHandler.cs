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

    public void SetActions(Action<float> onChange, Action<string[]> onInvalid, Action<float> onSubmit, Action<string[]> onAbort)
    {
        TextInputHandler.SetActions(
                (value) =>
                {
                    var numberValue = _numberInputSystem.ValidateNumberInput(value);
                    if (numberValue == null)
                    {
                        onInvalid?.Invoke(new[] {value});

                        TextInputHandler.SetBorderColorValid(false);
                    }
                    else
                    {
                        onChange?.Invoke(numberValue.Value);
                        TextInputHandler.SetBorderColorValid(true);
                    }
                },
                (value) =>
                {
                    var numberValue = _numberInputSystem.ValidateNumberInput(value);
                    if (numberValue == null)
                    {
                        onAbort?.Invoke(new[] {value});
                        TextInputHandler.ReturnPreviousSubmitValue();
                    }
                    else
                    {
                        onSubmit?.Invoke(numberValue.Value);
                    }
                },
                (value) =>
                {
                    //var numberValue = _numberInputSystem.ValidateNumberInput(value); // TODO
                    //if (numberValue == null)
                    //{
                    onAbort?.Invoke(new[] {value});
                    //}
                    //else
                    //{
                    //    onAbort?.Invoke(numberValue.Value);
                    //}
                });
    }

    private string NumberToString(float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public void SetDefaultColors()
    {
        TextInputHandler.SetDefaultInputColors();
    }
}

