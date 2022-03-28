using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialComponent : EntityComponent
{
    [Serializable]
    public class MaterialValues
    {
        public Color albedoColor = Color.white;
        public Color emissiveColor = Color.black;

        public MaterialValues Copy()
        {
            var copy = new MaterialValues
            {
                albedoColor = albedoColor,
                emissiveColor = emissiveColor
            };
            return copy;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(MaterialValues))
                return false;

            return
                obj is MaterialValues other &&
                other.albedoColor == albedoColor &&
                other.emissiveColor == emissiveColor;
        }

        public override int GetHashCode() // Overriding to shut up the compiler
        {
            return base.GetHashCode();
        }
    }

    [NonSerialized]
    public MaterialValues materialValues = new MaterialValues();

    public override string SpecificJson => JsonUtility.ToJson(materialValues);
    public override void ApplySpecificJson(string jsonString)
    {
        materialValues = JsonUtility.FromJson<MaterialValues>(jsonString);
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.MaterialComponentInScene, entity.componentsParent.transform);
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.MaterialComponentUI;
    public override string ComponentName => "material";
    public override int InspectorOrder => 150;
    public override Ts? GetTypeScript()
    {
        return new Ts(InternalComponentSymbol, $"const {InternalComponentSymbol} = new Material()\n" +
                                               $"{InternalComponentSymbol}.albedoColor = {materialValues.albedoColor.ToTS()}\n" +
                                               $"{InternalComponentSymbol}.emissiveColor = {materialValues.emissiveColor.ToTS()}\n");

    }
}
