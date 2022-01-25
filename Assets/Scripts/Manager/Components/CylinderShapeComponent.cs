using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderShapeComponent : EntityComponent
{
    public override string SpecificJson => "";
    public override void ApplySpecificJson(string jsonString)
    {
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.CylinderShapeComponentInScene, entity.componentsParent.transform);
        //entity.gameObject.AddComponent<BoxCollider>();
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.CylinderShapeComponentUI;

    public override string ComponentName => "cylinderShape";
    public override int InspectorOrder => 100;

    public override Ts GetTypeScript()
    {
        return new Ts( InternalComponentSymbol, $"const {InternalComponentSymbol} = new CylinderShape()\n");
    }
}

