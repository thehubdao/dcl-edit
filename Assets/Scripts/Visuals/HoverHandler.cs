using Assets.Scripts.EditorState;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const int CharacterWrapLimit = 40;
    [SerializeField][TextArea(5, 30)] private string text;
    [SerializeField] private float timeToAppear = 0.5f;
    [SerializeField] private bool appearAtObjectCenter = false;
    [Tooltip("Adds automatically new lines if the text is too long.")]
    [SerializeField] private bool autoWrap = false;
    private UnityState unityState;
    private GameObject currentHoverLabel;

    [Inject]
    void Construct(UnityState unityState)
    {
        this.unityState = unityState;
    }

    private IEnumerator CreateHoverLabel()
    {
        Vector2 mousePosition = Vector2.zero;
        float time = 0;
        while (time < timeToAppear)
        {
            if (mousePosition.x != Input.mousePosition.x || mousePosition.y != Input.mousePosition.y)
            {
                mousePosition = Input.mousePosition;
                time = 0;
            }
            else
                time += Time.deltaTime;
            yield return null;
        }

        currentHoverLabel = Instantiate(unityState.HoverLabel, unityState.Ui.transform);
        var rectTransform = currentHoverLabel.GetComponent<RectTransform>();
        var textField = currentHoverLabel.GetComponentInChildren<TMP_Text>();
        var canvasGroup = currentHoverLabel.GetComponent<CanvasGroup>();
        var layoutElement = currentHoverLabel.GetComponent<LayoutElement>();
        textField.text = text;
        float pivotX = Input.mousePosition.x < Screen.width / 2 ? 0 : 1;
        float pivotY = Input.mousePosition.y < Screen.height / 2 ? 0 : 1f;
        rectTransform.pivot = new(pivotX, pivotY);
        rectTransform.position = appearAtObjectCenter ? transform.position : new Vector2(Input.mousePosition.x, pivotY == 0 ? Input.mousePosition.y : Input.mousePosition.y - 25);
        layoutElement.enabled = autoWrap && text.Length > CharacterWrapLimit;
        
        time = 0;
        while(time <= 0.2f)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = time * 5;
            yield return null;
        }
    }

    private void DestroyHoverLabel()
    {
        Destroy(currentHoverLabel);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(CreateHoverLabel());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        DestroyHoverLabel();
    }
}
