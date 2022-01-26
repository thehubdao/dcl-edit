using System.Collections.Generic;
using UnityEngine;

public class MaterialComponentRepresentation : MonoBehaviour
{

    void Start()
    {
        UpdateVisuals();
        SceneManager.OnUpdateHierarchy.AddListener(UpdateVisuals);
        SceneManager.OnUpdateSelection.AddListener(UpdateVisuals);
    }

    private static bool EntityHasBasicShape(Entity e)
    {
        if (e == null)
            return false;


        return
            e.TryGetComponent(out BoxShapeComponent _) ||
            e.TryGetComponent(out SphereShapeComponent _) ||
            e.TryGetComponent(out PlaneShapeComponent _) ||
            e.TryGetComponent(out CylinderShapeComponent _) ||
            e.TryGetComponent(out ConeShapeComponent _);
    }

    private void UpdateVisuals()
    {
        var entity = GetComponentInParent<Entity>();

        if (EntityHasBasicShape(entity))
        {
            if (entity.TryGetComponent(out MaterialComponent materialComponent))
            {
                var allRenderer = entity.componentsParent.GetComponentsInChildren<Renderer>();
                ApplyMaterialToAllRenderer(allRenderer,materialComponent.materialValues);
            }
            else
            {
                var allRenderer = entity.componentsParent.GetComponentsInChildren<Renderer>();
                ApplyMaterialToAllRenderer(allRenderer,new MaterialComponent.MaterialValues());
            }
        }
    }

    private static void ApplyMaterialToAllRenderer(IEnumerable<Renderer> allRenderer, MaterialComponent.MaterialValues material)
    {
        foreach (var r in allRenderer)
        {
            r.material.SetColor("_BaseColor", material.albedoColor);
            r.material.SetColor("_EmissionColor", material.emissiveColor);
            r.material.EnableKeyword("_EMISSION");
        }
    }
}
