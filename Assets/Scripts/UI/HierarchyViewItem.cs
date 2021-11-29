using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HierarchyViewItem : MonoBehaviour,ISerializedFieldToStatic
{
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

    public void SelectEntity()
    {
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

}
