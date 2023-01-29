using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuBarButtonHandler : MonoBehaviour
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
