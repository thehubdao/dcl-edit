using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverLabelUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _labelObject;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private float _closeLabelAfterDistance = 5;

    


    private Vector3 _openedMousePos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        HoverLabelManager.onOpenLabel.AddListener(OpenLabel);
    }

    void Update()
    {
        if (_labelObject.activeSelf && Vector3.Distance(_openedMousePos, Input.mousePosition) > _closeLabelAfterDistance)
        {
            CloseLabel();
        }
    }

    private void OpenLabel(string text)
    {
        _labelObject.SetActive(true);
        _labelObject.GetComponent<RectTransform>().pivot = new Vector2(
            Input.mousePosition.x < Screen.width-300 ? -0.05f : 1.05f, 
            1.5f);

        _labelObject.transform.position = Input.mousePosition;
        _text.text = text;

        _openedMousePos = Input.mousePosition;
    }

    private void CloseLabel()
    {
        _labelObject.SetActive(false);
    }
}