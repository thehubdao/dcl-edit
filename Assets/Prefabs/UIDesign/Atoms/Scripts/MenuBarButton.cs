using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class MenuBarButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private TMP_Text titleText;

    public void Initialize(string title, UnityAction action)
    {
        titleText.text = title;
        button.onClick.AddListener(action);
    }
}
