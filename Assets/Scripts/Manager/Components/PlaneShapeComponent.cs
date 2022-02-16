using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneShapeComponent : EntityComponent
{
    public override string SpecificJson => "";
    public override void ApplySpecificJson(string jsonString)
    {
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.PlaneShapeComponentInScene, entity.componentsParent.transform);
        //entity.gameObject.AddComponent<BoxCollider>();
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.PlaneShapeComponentUI;

    public override string ComponentName => "planeShape";
    public override int InspectorOrder => 100;

    public override Ts? GetTypeScript()
    {
        return new Ts( InternalComponentSymbol, $"const {InternalComponentSymbol} = new PlaneShape()\n");
    }
}

