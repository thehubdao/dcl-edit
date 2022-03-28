using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewPortSetter : MonoBehaviour
{
    public RectTransform Gui;

    private int _lastScreenWidth; 
    private Camera _cam;

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        _lastScreenWidth = Screen.width;
        SetViewport();
    }

    public void SetViewport()
    {
        _cam.rect = new Rect(Gui.sizeDelta.x / Screen.width, 0, (Screen.width - Gui.sizeDelta.x) / Screen.width,
            1);
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastScreenWidth != Screen.width)
        {
            SetViewport();
            _lastScreenWidth = Screen.width;
        }
    }
}
