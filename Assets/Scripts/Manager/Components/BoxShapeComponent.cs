using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxShapeComponent : EntityComponent
{
    public override string SpecificJson => "";
    public override void ApplySpecificJson(string jsonString)
    {
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.BoxShapeComponentInScene, entity.componentsParent.transform);
        //entity.gameObject.AddComponent<BoxCollider>();
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.BoxShapeComponentUI;

    public override string ComponentName => "boxShape";

    public override Ts GetTypeScript()
    {
        return new Ts( InternalComponentSymbol, $"const {InternalComponentSymbol} = new BoxShape()\n");
    }
}

