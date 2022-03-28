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
        catch (AssetManager.AssetNotFoundException)
        {
            asset = null;
        }
    }

    //public string glbPath;
    public AssetManager.GLTFAsset asset;

    public override string ComponentName => "GLTFShape";
    public override int InspectorOrder => 100;
    
    // This is not good. TODO: change, when refactoring script generator
    private static Dictionary<string, string> _availableGltfShapes = new Dictionary<string, string>();

    // resets the shape cache. This has to be called before every script generation
    public static void ClearShapeCache()
    {
        _availableGltfShapes = new Dictionary<string, string>();
    }

    public override Ts? GetTypeScript()
    {
        if (asset == null)
            return null;

        if (_availableGltfShapes.ContainsKey(asset.gltfPath))
        {
            return new Ts(_availableGltfShapes[asset.gltfPath], "");
        }

        _availableGltfShapes.Add(asset.gltfPath, InternalComponentSymbol);

        return new Ts(InternalComponentSymbol, $"const {InternalComponentSymbol} = new GLTFShape(\"{asset.gltfPath}\")\n" +
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
