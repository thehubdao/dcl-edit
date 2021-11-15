using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtendDown : MonoBehaviour
{
    [SerializeField]
    private Button _button = default;
    public void UpdateVisuals()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, _button.interactable ? 2 : 0);
    }
}
