using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public static float UiScale
    {
        get => PersistentData.UiScale;
        set
        {
            PersistentData.UiScale = value;
            PersistentData.UiScale = PersistentData.UiScale.Clamp(0.5f, 3f);
            onUiChange.Invoke();
        }
    }

    public static UnityEvent onUiChange = new UnityEvent();
}
