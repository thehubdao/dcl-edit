using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverLabel : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private const float _hoverDelay = 0.5f;

    [SerializeField]
    private string _text;
    

    private float _hoverTimer = float.PositiveInfinity;
    private Vector3 _lastMousePosition = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_hoverTimer < 1000000)
        {
            _hoverTimer -= Time.deltaTime;

            if (_lastMousePosition != Input.mousePosition)
            {
                _hoverTimer = _hoverDelay;
            }

            if (_hoverTimer <= 0)
            {
                HoverLabelManager.OpenLabel(_text);
            }

            _lastMousePosition = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverTimer = _hoverDelay;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverTimer = float.PositiveInfinity;
    }
}
