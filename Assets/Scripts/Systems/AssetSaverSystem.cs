using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AssetSaverSystem : MonoBehaviour
{
    public bool saveNow = false;
    public bool loadNow = false;

    void Update()
    {
        if(saveNow)
        {
            saveNow = false;
            Save();
        }
        if(loadNow)
        {
            loadNow = false;
            Load();
        }
    }

    [Serializable]
    private class AssetsJsonWrapper
    {
        [Serializable]
        public struct GltfAssetWrapper
        {
            public GltfAssetWrapper(AssetManager.GLTFAsset asset)
            {
                name = asset.name;
                id = asset.id.ToString();    
                gltfPath = asset.gltfPath;
            }

            public string name;
            public string id;
            public string gltfPath;
        }

        public AssetsJsonWrapper()
        {
            gltfAssets = AssetManager.AllGltfAssets
                .Select(asset => new GltfAssetWrapper(asset))
                .ToList();
        }

        public List<GltfAssetWrapper> gltfAssets;
    }


    public static void Save()
    {
        var assetsJsonWrapper = new AssetsJsonWrapper();
        var jsonString = JsonUtility.ToJson(assetsJsonWrapper,true);

        Directory.CreateDirectory(DclSceneManager.DclProjectPath + "/dcl-edit/saves");
        try
        {
            var fileWriter = new StreamWriter(DclSceneManager.DclProjectPath + "/dcl-edit/saves/assets.json", false);

            fileWriter.WriteLine(jsonString);

            fileWriter.Close();
        }
        catch (IOException)
        {
            Debug.LogError("Error while saving assets");
        }
    }

    public static void Load()
    {
        AssetManager.allAssets = new List<AssetManager.Asset>();

        if (File.Exists(DclSceneManager.DclProjectPath + "/dcl-edit/saves/assets.json"))
        {
            var fileContent = File.ReadAllText(DclSceneManager.DclProjectPath + "/dcl-edit/saves/assets.json");
            var assetsJsonWrapper = JsonUtility.FromJson<AssetsJsonWrapper>(fileContent);

            foreach (var gltfAsset in assetsJsonWrapper.gltfAssets)
            {
                AssetManager.allAssets.Add(new AssetManager.GLTFAsset(gltfAsset.name, System.Guid.Parse(gltfAsset.id) ,gltfAsset.gltfPath));
            }
        }
    }
}
