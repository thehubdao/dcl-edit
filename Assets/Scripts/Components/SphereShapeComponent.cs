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
        Instantiate(ComponentRepresentationList.SphereShapeComponent, entity.componentsParent.transform);
        var spc = entity.gameObject.AddComponent<SphereCollider>();
        spc.radius = 1;
    }

    public override string ComponentName => "sphereShape";

    public override Ts GetTypeScript()
    {
        return new Ts( $"{entity.name.ToCamelCase()}SphereShape", $"const {entity.name.ToCamelCase()}SphereShape = new SphereShape()\n");
    }
}

