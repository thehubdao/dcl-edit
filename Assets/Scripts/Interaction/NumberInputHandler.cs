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

    private NumberInputSystem numberInputSystem;

    [Inject]
    private void Construct(NumberInputSystem numberInputSystem)
    {
        this.numberInputSystem = numberInputSystem;
    }

    public void SetCurrentNumber(float currentContents)
    {
        TextInputHandler.SetCurrentText(NumberToString(currentContents));
    }

    public float? GetCurrentNumber()
    {
        return numberInputSystem.ValidateNumberInput(TextInputHandler.GetCurrentText());
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
                var numberValue = numberInputSystem.ValidateNumberInput(value);
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
                var numberValue = numberInputSystem.ValidateNumberInput(value);
                if (numberValue == null)
                {
                    onAbort?.Invoke(new[] {value});
                }
                else
                {
                    onSubmit?.Invoke(numberValue.Value);
                }
            },
            (value) => { onAbort?.Invoke(new[] {value}); });
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

