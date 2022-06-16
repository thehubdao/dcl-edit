using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class AddRenderTexture : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        UpdateRenderTexture();
    }

    private int lastWidth = -1;
    private int lastHeight = -1;

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateRenderTexture();
        }
    }


    private void UpdateRenderTexture()
    {
        cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 1, DefaultFormat.LDR);
        cam.forceIntoRenderTexture = true;

        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
}
