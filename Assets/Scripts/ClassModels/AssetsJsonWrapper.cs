using System;
using System.Collections.Generic;

namespace Assets.Scripts.ClassModels
{
    [Serializable]
    public class AssetsJsonWrapper
    {
        public List<GltfAssetWrapper> gltfAssets;
    }
    
    [Serializable]
    public struct GltfAssetWrapper
    {
        public string name;
        public string id;
        public string gltfPath;
    }
}