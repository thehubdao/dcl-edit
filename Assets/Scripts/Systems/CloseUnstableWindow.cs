using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CloseUnstableWindow : MonoBehaviour
{
    private RectTransform rectTransform;

    public UnityEvent OnClose;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) ||
            Input.GetMouseButtonDown((int)MouseButton.RightMouse))
        {
            var worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            Rect r = new Rect(worldCorners[0], worldCorners[2] - worldCorners[0]);

            //Debug.Log($"Rect: {r}, Mouse is in: {r.Contains(Input.mousePosition)}");

            if (!r.Contains(Input.mousePosition))
            {
                gameObject.SetActive(false);
                OnClose.Invoke();
            }
        }
    }
}
