using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TextMeshProUGUI placeHolderText;

    public void SetCurrentText(string text)
    {
        inputField.text = text;
    }

    public void SetPlaceHolder(string placeHolder)
    {
        placeHolderText.text = placeHolder;
    }
}
