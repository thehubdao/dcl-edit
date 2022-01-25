using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeShapeComponent : EntityComponent
{
    public override string SpecificJson => "";
    public override void ApplySpecificJson(string jsonString)
    {
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.ConeShapeComponentInScene, entity.componentsParent.transform);
        //entity.gameObject.AddComponent<BoxCollider>();
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.ConeShapeComponentUI;

    public override string ComponentName => "coneShape";
    public override int InspectorOrder => 100;

    public override Ts GetTypeScript()
    {
        return new Ts( InternalComponentSymbol, $"const {InternalComponentSymbol} = new ConeShape()\n");
    }
}

