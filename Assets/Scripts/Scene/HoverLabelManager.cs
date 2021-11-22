using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoverLabelManager : Manager
{

    public static UnityEvent<string> onOpenLabel = new UnityEvent<string>();

    public static void OpenLabel(string text)
    {
        onOpenLabel.Invoke(text);
    }

}
