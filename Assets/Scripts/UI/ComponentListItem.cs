using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComponentListItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [NonSerialized]
    public Type componentType;

    [NonSerialized]
    public string componentName;

    public void UpdateVisuals()
    {
        _text.text = componentName;
    }

    public void AddComponent()
    {
        SceneManager.PrimarySelectedEntity.gameObject.AddComponent(componentType);
        GetComponentInParent<CloseUnstableWindow>().gameObject.SetActive(false);
        SceneManager.OnUpdateSelection.Invoke();
    }
}
