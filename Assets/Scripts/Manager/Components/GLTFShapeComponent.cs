using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLTFShapeComponent : EntityComponent
{
    class SpecificGltfShapeJson
    {
        public SpecificGltfShapeJson(GLTFShapeComponent sc)
        {
            assetID = sc.asset?.id.ToString() ?? Guid.Empty.ToString();
        }

        public string assetID;
    }

    public override string SpecificJson => JsonUtility.ToJson(new SpecificGltfShapeJson(this));
    public override void ApplySpecificJson(string jsonString)
    {
        var json = JsonUtility.FromJson<SpecificGltfShapeJson>(jsonString);
        //glbPath = json.glbPath;
        try
        {
            asset = AssetManager.GetAssetById<AssetManager.GLTFAsset>(System.Guid.Parse(json.assetID));
        }
        catch (AssetManager.AssetNotFoundException e)
        {
            asset = null;
        }
    }

    //public string glbPath;
    public AssetManager.GLTFAsset asset;

    public override string ComponentName => "GLTFShape";
    public override int InspectorOrder => 100;

    public override Ts? GetTypeScript()
    {
        if(asset == null)
            return null;
        
        return new Ts( InternalComponentSymbol, $"const {InternalComponentSymbol} = new GLTFShape(\"{asset.gltfPath}\")\n" +
                                                     $"{InternalComponentSymbol}.withCollisions = true\n" +
                                                     $"{InternalComponentSymbol}.isPointerBlocker = true\n" +
                                                     $"{InternalComponentSymbol}.visible = true\n");
    }

    public override void Start()
    {
        base.Start();
        componentRepresentation = Instantiate(ComponentRepresentationList.GltfShapeComponentInScene, entity.componentsParent.transform);
        //componentRepresentation.GetComponent<GltfComponentRepresentation>().UpdateVisuals(this);
    }

    public override GameObject UiItemTemplate => ComponentRepresentationList.GltfShapeComponentUI;
}
