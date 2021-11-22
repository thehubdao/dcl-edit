using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HierarchyViewItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    [NonSerialized]
    public Entity entity;

    [NonSerialized]
    public int indentLevel = 0;

    private bool IsSelected => entity == SceneManager.SelectedEntity;

    private static readonly Color _defaultWhite = new Color(0.8980393f, 0.9058824f, 0.9215687f);
    private static readonly Color _selectionBlue = new Color(0.5764706f, 0.7725491f, 0.9921569f);

    public void UpdateVisuals()
    {
        nameText.text = entity.ShownName;
        nameText.margin = new Vector4((indentLevel + 1) * 20f,0,0,0);
        nameText.color = IsSelected ? _selectionBlue : _defaultWhite;
    }

    public void SelectEntity()
    {
        SceneManager.SelectedEntity = entity;
    }
}
