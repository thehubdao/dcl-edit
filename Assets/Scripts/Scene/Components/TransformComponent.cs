using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class TransformComponent : EntityComponent
{
    [Serializable]
    public class SpecificTransformJson
    {
        public SpecificTransformJson(TransformComponent tc)
        {
            pos = tc.transform.position;
            rot = tc.transform.rotation;
            scale = tc.entity.componentsParent.transform.localScale;
        }

        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
    }

    public override string SpecificJson => JsonUtility.ToJson(new SpecificTransformJson(this));
    public override void ApplySpecificJson(string jsonString)
    {
        var specifics = JsonUtility.FromJson<SpecificTransformJson>(jsonString);
        transform.position = specifics.pos;
        transform.rotation = specifics.rot;
        // this is called, before start is called. Therefore we need to use GetComponent here
        GetComponent<Entity>().componentsParent.transform.localScale = specifics.scale;
    }


    public override void Start()
    {
        base.Start();

        // Setup representation
        componentRepresentation = Instantiate(ComponentRepresentationList.TransformComponentInScene, entity.componentsParent.transform);
    }


    public override GameObject UiItemTemplate => ComponentRepresentationList.TransformComponentUI;

    public override string ComponentName => "transform";

    public override Ts GetTypeScript()
    {
        //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

        var pos = transform.position;
        var rot = transform.rotation;
        var scale = entity.componentsParent.transform.localScale;

        return new Ts(InternalComponentSymbol,
            $"const {InternalComponentSymbol} = new Transform({{\n" +
            $"  position: {pos.ToTS()},\n" + 
            $"  rotation: {rot.ToTS()},\n" +
            $"  scale: {scale.ToTS()}\n" +
            $"}})\n");
    }
}
