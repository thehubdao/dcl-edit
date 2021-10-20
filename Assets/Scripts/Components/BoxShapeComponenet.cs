using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxShapeComponenet : EntityComponent
{
    public override void Start()
    {
        base.Start();
        Instantiate(ComponentRepresentationList.BoxShapeComponent, entity.componentsParent.transform);
        entity.gameObject.AddComponent<BoxCollider>();
    }

    public override Ts GetTypeScript()
    {
        return new Ts( $"{entity.name.ToCamelCase()}BoxShape", $"const {entity.name.ToCamelCase()}BoxShape = new BoxShape()\n");
    }
}

