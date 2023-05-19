using System;
using HSVPicker;
using UnityEngine;
using UnityEngine.Events;

public class OpenColorPickerWindowSystem : MonoBehaviour//, ISerializedFieldToStatic
{
    [SerializeField]
    private GameObject _colorPickerWindow;
    private static GameObject ColorPickerWindow;

    public void SetupStatics()
    {
        ColorPickerWindow = _colorPickerWindow;
    }

    public static void RequestColor(Color currentColor, Action<Color> onColorSelected)
    {
        // open color picker window
        ColorPickerWindow.SetActive(true);

        // setup color picker
        //var colorPicker = ColorPickerWindow.GetComponentInChildren<ColorPicker>();
        //var unstableWindow = ColorPickerWindow.GetComponent<CloseUnstableWindow>();

        // set current color
        //colorPicker.CurrentColor = currentColor;

        // wait for color to be picked
        //unstableWindow.OnClose = new UnityEvent();
        //unstableWindow.OnClose.AddListener(() =>
        //{
        //    onColorSelected(colorPicker.CurrentColor);
        //});
    }
}
