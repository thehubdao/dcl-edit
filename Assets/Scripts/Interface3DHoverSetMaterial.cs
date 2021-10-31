using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface3DHoverSetMaterial : Interface3DHover
{
    public MeshRenderer[] renderers;

    public override void StartHover()
    {
        foreach (var r in renderers)
        {
            r.material.SetFloat("hover", 1);
        }
    }

    public override void EndHover()
    {
        foreach (var r in renderers)
        {
            r.material.SetFloat("hover", 0);
        }
    }
}
