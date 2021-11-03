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
        Instantiate(ComponentRepresentationList.BoxShapeComponent, entity.componentsParent.transform);
        //entity.gameObject.AddComponent<BoxCollider>();
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.BoxShapeComponentUI;

    public override string ComponentName => "boxShape";

    public override Ts GetTypeScript()
    {
        return new Ts( $"{entity.NameTsSymbol.ToCamelCase()}BoxShape", $"const {entity.NameTsSymbol.ToCamelCase()}BoxShape = new BoxShape()\n");
    }
}

