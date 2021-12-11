using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereShapeComponent : EntityComponent
{
    public override string SpecificJson => "";
    public override void ApplySpecificJson(string jsonString)
    {
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.SphereShapeComponentInScene, entity.componentsParent.transform);
        //var spc = entity.gameObject.AddComponent<MeshCollider>();
        //spc.sharedMesh = ComponentRepresentationList.SphereShapeComponent.GetComponentInChildren<MeshFilter>().sharedMesh;
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.SphereShapeComponentUI;

    public override string ComponentName => "sphereShape";
    public override int InspectorOrder => 100;

    public override Ts GetTypeScript()
    {
        return new Ts( InternalComponentSymbol, $"const {InternalComponentSymbol} = new SphereShape()\n");
    }
}

