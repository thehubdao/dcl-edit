using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxShapeComponenet : EntityComponent
{
    public override Ts GetTypeScript(string entityName)
    {
        return new Ts( $"{entityName}BoxShape", $"const {entityName}BoxShape = new BoxShape()\n");
    }
}

