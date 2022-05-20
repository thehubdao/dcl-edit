using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    public Guid Id;

    public void UpdateVisuals()
    {
        var entity = EditorStates.CurrentSceneState.CurrentScene?.GetEntityFormId(Id);
        if (entity == null)
            return;

        var transformComponent = entity.GetComponentByName("transform");
        if (transformComponent != null)
        {
            transform.position = transformComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value;
            transform.rotation = transformComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value;
            transform.localScale = transformComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value;
        }
    }
}
