using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickAnywhereToExit : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
        {
            Application.Quit();
        }
    }
}
