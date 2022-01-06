using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyViewItem : MonoBehaviour,ISerializedFieldToStatic,IPointerClickHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [Header("Pointer Handler")]
    [Space(20,order = 100)]

    public TextMeshProUGUI nameText;

    [NonSerialized]
    public Entity entity;

    [NonSerialized]
    public int indentLevel = 0;

    private bool IsPrimarySelected => entity == SceneManager.PrimarySelectedEntity;
    private bool IsSecondarySelected => SceneManager.SecondarySelectedEntity.Contains(entity);


    [SerializeField]
    private Color defaultWhite = new Color(0.8980393f, 0.9058824f, 0.9215687f);

    [SerializeField]
    private Color floatingColor = new Color(0.8980393f, 0.9058824f, 0.9215687f);

    [SerializeField]
    private Color primarySelect = new Color(0.5764706f, 0.7725491f, 0.9921569f);

    [SerializeField]
    private Color secondarySelect = new Color(0.5764706f, 0.7725491f, 0.9921569f);
    

    private static Color _defaultWhite;
    private static Color _primarySelectionBlue;
    private static Color _secondarySelectionBlue;

    public void SetupStatics()
    {
        _defaultWhite = defaultWhite;
        _primarySelectionBlue = primarySelect;
        _secondarySelectionBlue = secondarySelect;
    }

    public void UpdateVisuals()
    {
        nameText.text = entity.ShownName;
        nameText.margin = new Vector4((indentLevel + 1) * 20f,0,0,0);
        nameText.color = 
            IsPrimarySelected ? _primarySelectionBlue : 
            IsSecondarySelected ? _secondarySelectionBlue : _defaultWhite;
    }

    public void MoveBefore(Entity entity)
    {
        Debug.Log("Move before "+entity.ShownName);
    }

    public void MoveAfter(Entity entity)
    {
        Debug.Log("Move after "+entity.ShownName);
    }

    public void MoveToChild(Entity entity)
    {
        Debug.Log("Move to child of "+entity.ShownName);
    }

    public void SelectEntity()
    {
        /*Instantiate(gameObject, CanvasManager.FloatingListItemParent.transform, true);*/
        var pressingControl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (!pressingControl)
        {
            SceneManager.SetSelection(entity);
        }
        else
        {
            SceneManager.AddSelection(entity);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");
        SelectEntity();
    }

    private GameObject _dragCopy = null;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        GetComponentInParent<HierarchyView>().EnableDropZones();

        _dragCopy = Instantiate(gameObject, CanvasManager.FloatingListItemParent.transform, true);
        var cg = _dragCopy.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        
        nameText.color = floatingColor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        
        GetComponentInParent<HierarchyView>().DisableDropZones();

        Destroy(_dragCopy);
        _dragCopy = null;
        
        nameText.color = 
            IsPrimarySelected ? _primarySelectionBlue : 
            IsSecondarySelected ? _secondarySelectionBlue : _defaultWhite;

        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if(_dragCopy != null)
        {
            _dragCopy.GetComponent<RectTransform>().anchoredPosition += eventData.delta / CanvasManager.MainCanvas.scaleFactor;
        }
    }

    [SerializeField]
    private GameObject _dropZone;
    
    public void SetDropZoneActive(bool value)
    {
        _dropZone.SetActive(value);
    }
}
