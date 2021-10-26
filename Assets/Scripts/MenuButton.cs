using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject showGameObject;
    public Button thisButton;

    public GameObject[] hideGameObjects;
    public Button[] otherButtons;

    public void Clicked()
    {
        thisButton.interactable = false;
        showGameObject.SetActive(true);

        foreach (var hideGameObject in hideGameObjects)
        {
            hideGameObject.SetActive(false);
        }

        foreach (var otherButton in otherButtons)
        {
            otherButton.interactable = true;
        }
    }
}
