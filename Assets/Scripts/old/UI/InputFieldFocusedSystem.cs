using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputFieldFocusedSystem : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        CanvasManager.IsAnyInputFieldFocused = 
            GetComponentsInChildren<TMP_InputField>()
                .Any(field => field.isFocused);
    }
}
