using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TransformComponenet : EntityComponent
{


    public override Ts GetTypeScript(string entityName)
    {
        //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

        var pos = transform.position;
        var rot = transform.rotation;
        var scale = transform.localScale;

        return new Ts($"{entityName}Transform",
            $"const {entityName}Transform = new Transform({{\n" +
            $"  position: {pos.ToTS()},\n" +
            $"  rotation: {rot.ToTS()},\n" +
            $"  scale: {scale.ToTS()}\n" +
            $"}})\n");
    }
}
