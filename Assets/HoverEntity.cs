using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HoverEntity : Interface3DHover
{
    public Material material;
    public MeshFilter mesh;

    private bool _isHovering = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_isHovering)
            Graphics.DrawMesh(mesh: mesh.mesh,position: mesh.transform.position,rotation: mesh.transform.rotation,material: material,layer: LayerMask.NameToLayer("Entity"));
    }

    public override void StartHover()
    {
        _isHovering = true;
    }

    public override void EndHover()
    {
        _isHovering = false;
    }
}
