using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;

public class TextInputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TextMeshProUGUI placeHolderText;

    private Color _currentHighlightedBorderColor;
    private Color _validHighlightedBorderColor;

    // dependencies
    private InputState _inputState;

    [Inject]
    private void Construct(InputState inputState)
    {
        _inputState = inputState;
        _validHighlightedBorderColor = inputField.colors.selectedColor;
        _currentHighlightedBorderColor = _validHighlightedBorderColor;
    }

    public void SetCurrentText(string text)
    {
        inputField.text = text;
    }

    public string GetCurrentText()
    {
        return inputField.text;
    }

    public void SetPlaceHolder(string placeHolder)
    {
        placeHolderText.text = placeHolder;
    }

    public void ResetActions()
    {
        // remove old listeners. This is necessary, because the input field is pooled and might be reused
        inputField.onSelect.RemoveAllListeners();
        inputField.onValueChanged.RemoveAllListeners();
        inputField.onEndEdit.RemoveAllListeners();
    }

    public void SetActions([CanBeNull] Action<string> onChange, [CanBeNull] Action<string> onSubmit, [CanBeNull] Action<string> onAbort)
    {
        ResetActions();

        inputField.onSelect.AddListener(_ => _inputState.InState = InputState.InStateType.UiInput);
        inputField.onValueChanged.AddListener(value => onChange?.Invoke(value));
        inputField.onEndEdit.AddListener(value =>
        {
            _inputState.InState = InputState.InStateType.NoInput;
            if (inputField.wasCanceled)
            {
                //Debug.Log("Aborting input");
                onAbort?.Invoke(value);
            }
            else
            {
                //Debug.Log("Submitting input");
                onSubmit?.Invoke(value);
            }
        });
    }

    public void SetBorderColorValid(bool validity)
    {
        var color = validity ? _validHighlightedBorderColor : Color.red;
        
        if (_currentHighlightedBorderColor.Equals(color))
            return;
        
        _currentHighlightedBorderColor = color;
        var inputFieldColors = inputField.colors;
        inputFieldColors.selectedColor = color;
        inputField.colors = inputFieldColors;
    }

    public void SetDefaultInputColors()
    {
        if (_currentHighlightedBorderColor.Equals(_validHighlightedBorderColor))
            return;

        _currentHighlightedBorderColor = _validHighlightedBorderColor;
        var inputFieldColors = inputField.colors;
        inputFieldColors.selectedColor = _validHighlightedBorderColor;
        inputField.colors = inputFieldColors;
    }
}

