using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common;
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

    public void UpdateVisuals()
    {
        nameText.text = entity.Name;
        nameText.margin = new Vector4((indentLevel + 1) * 20f,0,0,0);
        nameText.color = IsSelected ? Color.blue : Color.black;
    }

    public void SelectEntity()
    {
        SceneManager.SelectedEntity = entity;
    }
}
