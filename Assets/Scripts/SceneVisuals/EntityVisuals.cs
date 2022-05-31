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

        // Transform
        var transformComponent = entity.GetComponentByName("transform");
        if (transformComponent != null)
        {
            transform.position = transformComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value;
            transform.rotation = transformComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value;
            transform.localScale = transformComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value;
        }

        // GLTF Shape
        var gltfShapeComponent = entity.GetComponentByName("GLTFShape");
        var gltfShapeVisualization = GetComponent<GltfShapeVisualization>(); // returns null if component isn't found
        if (gltfShapeComponent != null)
        {
            if (gltfShapeVisualization == null)
                gltfShapeVisualization = gameObject.AddComponent<GltfShapeVisualization>();

            gltfShapeVisualization.UpdateVisuals(entity);
        }
        else if (gltfShapeVisualization != null)
        {
            gltfShapeVisualization.Deactivate();
        }
    }
}
