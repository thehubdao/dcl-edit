using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUI : MonoBehaviour
{
    [SerializeField]
    private CanvasScaler _canvasScaler;

    void Awake()
    {
        CanvasManager.onUiChange.AddListener(
            () => _canvasScaler.scaleFactor = CanvasManager.UiScale
            );
    }
}
