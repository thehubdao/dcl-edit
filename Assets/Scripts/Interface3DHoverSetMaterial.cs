using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface3DHoverSetMaterial : Interface3DHover
{
    public MeshRenderer renderer;

    public override void StartHover()
    {
        renderer.material.SetFloat("hover", 1);
    }

    public override void EndHover()
    {
        renderer.material.SetFloat("hover", 0);
    }
}
