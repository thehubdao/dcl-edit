using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Manager, ISerializedFieldToStatic
{
    public static bool IsAnyInputFieldFocused { get; set; }

    [SerializeField]
    private GameObject floatingListItemParent;

    public static GameObject FloatingListItemParent { get; set; }

    [SerializeField]
    private Canvas mainCanvas;

    public static Canvas MainCanvas { get; set; }

    public void SetupStatics()
    {
        FloatingListItemParent = floatingListItemParent;
        MainCanvas = mainCanvas;
    }
}
