using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEntity : Interface3DHover
{
    public Material material;
    //public MeshFilter mesh;
    public GameObject components;

    private bool _isHovering = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_isHovering)
            foreach (var meshFilter in components.GetComponentsInChildren<MeshFilter>())
                Graphics.DrawMesh(mesh: meshFilter.mesh,position: meshFilter.transform.position,rotation: meshFilter.transform.rotation,material: material,layer: LayerMask.NameToLayer("Entity"));
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
