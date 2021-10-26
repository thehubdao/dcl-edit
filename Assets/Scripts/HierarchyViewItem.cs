using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HierarchyViewItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    [NonSerialized]
    public new string name;
    public void UpdateVisuals()
    {
        nameText.text = name;
    }
}
