using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualRepresentation : MonoBehaviour
{
    private bool _dirty;

    public void SetDirty()
    {
        _dirty = true;
    }

    public abstract void UpdateVisuals();
    
    void OnEnable()
    {
        SetDirty();
        //Debug.Log("Enabled "+gameObject,gameObject);
    }

    void LateUpdate()
    {
        if (_dirty)
        {
            _dirty = false;
            UpdateVisuals();
        }
    }
}
