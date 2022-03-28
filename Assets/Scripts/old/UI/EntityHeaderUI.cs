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
        _nameInput.text = entity.CustomName;
    }


    public void SetEntityName()
    {
        entity.CustomName = _nameInput.text;
        DclSceneManager.OnUpdateSelection.Invoke();
    }

    // Undo stuff

    private string _oldName = "";

    public void StartUndoRecording()
    {
        _oldName = entity.CustomName;
    }

    public void EndUndoRecording()
    {
        if (_oldName != entity.CustomName)
        {
            var newName = entity.CustomName;
            var oldName = _oldName;
            UndoManager.RecordUndoItem($"Name changed from {oldName} to {newName}",
                () =>
                {
                    entity.CustomName = oldName;
                    DclSceneManager.OnUpdateSelection.Invoke();
                },
                () =>
                {
                    entity.CustomName = newName;
                    DclSceneManager.OnUpdateSelection.Invoke();
                });
        }
    }

}
