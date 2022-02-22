using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using UnityEngine;

public class AssetAutoDetectionSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DetectGltfAssets();
    }

    public static void DetectGltfAssets()
    {
        var paths = Directory.GetFiles(DclSceneManager.DclProjectPath, "*.glb", SearchOption.AllDirectories);
        var projectUri = new Uri(DclSceneManager.DclProjectPath);
        

        foreach (var path in paths)
        {
            var relativePath = path.Replace(DclSceneManager.DclProjectPath+"\\", "").Replace("\\","/");

            if(relativePath.StartsWith("node_modules"))
                continue;
            
            var pathParts = relativePath.Split('/');
            var fileName = pathParts[pathParts.Length - 1].Replace(".glb","");

            if(!AssetManager.AllGltfAssets.Select(asset => asset.gltfPath).Contains(relativePath))
            {
                Debug.Log("Added new asset " + fileName.ToHumanName());
                

                AssetManager.allAssets.Add(new AssetManager.GLTFAsset(fileName.ToHumanName(), System.Guid.NewGuid(), relativePath));
            }
        }

        AssetManager.OnAssetChange.Invoke();
    }
    

}
