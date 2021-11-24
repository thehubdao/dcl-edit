using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetManager : Manager
{
    public class Asset
    {
        public Asset(string name)
        {
            this.name = name;
        }

        public string name;

        public override string ToString()
        {
            return this.name + " " + base.ToString();
        }
    }

    public class GLTFAsset : Asset
    {
        public GLTFAsset(string name, string gltfPath) : base(name)
        {
            this.gltfPath = gltfPath;
        }

        public string gltfPath;
    }

    public static List<Asset> allAssets = new List<Asset>();

    public static IEnumerable<GLTFAsset> AllGltfAssets => 
        allAssets
        .Where(asset => asset.GetType() == typeof(GLTFAsset))
        .Select(asset => (GLTFAsset)asset);
    
}
