using System;
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

    public void SetActions(Action<string> onChange, Action<string> onSubmit, Action<string> onAbort)
    {
        // remove old listeners. This is necessary, because the input field is pooled and might be reused
        inputField.onSelect.RemoveAllListeners();
        inputField.onValueChanged.RemoveAllListeners();
        inputField.onEndEdit.RemoveAllListeners();

        inputField.onSelect.AddListener(_ => _inputState.InState = InputState.InStateType.UiInput);
        inputField.onValueChanged.AddListener(value => onChange(value));
        inputField.onEndEdit.AddListener(value =>
        {
            _inputState.InState = InputState.InStateType.NoInput;
            if(inputField.wasCanceled)
            {
                //Debug.Log("Aborting input");
                onAbort(value);
            }
            else
            {
                //Debug.Log("Submitting input");
                onSubmit(value);
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
}

