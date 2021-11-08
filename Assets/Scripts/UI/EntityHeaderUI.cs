using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityHeaderUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _nameInput;

    [NonSerialized]
    public Entity entity;

    public void UpdateVisuals()
    {
        _nameInput.text = entity.customName;
    }

    public void SetEntityName()
    {
        entity.customName = _nameInput.text;
        SceneManager.OnUpdateSelection.Invoke();
    }

}
