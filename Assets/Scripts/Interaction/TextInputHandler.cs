using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using TMPro;
using UnityEngine;
using Zenject;

public class TextInputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TextMeshProUGUI placeHolderText;

    // dependencies
    private InputState _inputState;

    [Inject]
    private void Construct(InputState inputState)
    {
        _inputState = inputState;
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
}
