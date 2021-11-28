using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AssetManager : Manager
{
    public class Asset
    {
        public Asset(string name,System.Guid id)
        {
            this.name = name;
            this.id = id;
        }

        public string name;
        public System.Guid id;
        public string TypeName { get; protected set; } = "Asset";

        public override string ToString()
        {
            return this.name + " " + base.ToString();
        }
    }

    public class GLTFAsset : Asset
    {
        public GLTFAsset(string name,System.Guid id , string gltfPath) : base(name,id)
        {
            //Debug.Log(id);
            TypeName = "GLTF";
            this.gltfPath = gltfPath;
        }

        public string gltfPath;
    }

    public static UnityEvent OnAssetChange = new UnityEvent();

    public static List<Asset> allAssets = new List<Asset>();

    public static IEnumerable<GLTFAsset> AllGltfAssets => 
        allAssets
        .Where(asset => asset.GetType() == typeof(GLTFAsset))
        .Select(asset => (GLTFAsset)asset);
    
    public static T GetAssetById<T>(System.Guid id) where T:Asset
    {
        var asset = GetAssetById(id);

        if (asset == null)
            throw new System.Exception("Asset not found");

        return (T)asset;
        
    }

    public static Asset GetAssetById(System.Guid id)
    {
        foreach(var asset in allAssets)
        {
            if (asset.id == id)
                return asset;
        }
        return null;
    }
}
