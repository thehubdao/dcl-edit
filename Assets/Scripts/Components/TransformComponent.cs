using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Constraints;
using UnityEngine;

public class TransformComponent : EntityComponent
{
    public override void Start()
    {
        base.Start();

        // Setup representation
        Instantiate(ComponentRepresentationList.TransformComponent, entity.componentsParent.transform);
    }

    public override string ComponentName => "transform";

    public override Ts GetTypeScript()
    {
        //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

        var pos = transform.position;
        var rot = transform.rotation;
        var scale = transform.localScale;

        return new Ts($"{entity.name.ToCamelCase()}Transform",
            $"const {entity.name.ToCamelCase()}Transform = new Transform({{\n" +
            $"  position: {pos.ToTS()},\n" +
            $"  rotation: {rot.ToTS()},\n" +
            $"  scale: {scale.ToTS()}\n" +
            $"}})\n");
    }
}
