using UnityEngine;

public class UiLoadingSymbolVisuals : MonoBehaviour
{
    private RectTransform rect;
    private float rotateSpeed = 200f;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
    }
}
