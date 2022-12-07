using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Zenject;
using static Assets.Scripts.System.SceneLoadSaveSystem;

public class SceneJsonReader
{
    private static DecentralandSceneData decentralandSceneData;

    // Dependencies
    private IPathState _pathState;

    [Inject]
    private void Construct(IPathState pathState)
    {
        _pathState = pathState;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Loads the Scene Data that the user works on. Loads it just once and stores it as a static variable
    /// </summary>
    /// <param name="reload">if it should reload the scene data from the json file again</param>
    /// <returns>the Decentraland scene data; if there is an error it returns null</returns>
    public DecentralandSceneData getSceneData(bool reload)
    {
        try
        {
            var json = File.ReadAllText(_pathState.ProjectPath + "/scene.json");

            if (reload == true || decentralandSceneData == null)
            {
                decentralandSceneData = JsonConvert.DeserializeObject<DecentralandSceneData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return null;
        }

        return decentralandSceneData;
    }

    /// <summary>
    /// Stores the Decentraland Scene Data
    /// </summary>
    public class DecentralandSceneData
    {
        public Dis display { get; set; }
        public string owner { get; set; }
        public Con contact { get; set; }

        public string main { get; set; }

        public List<string> tags { get; set; }

        public Sce scene { get; set; }

        public List<Spawn> spawnPoints { get; set; }

        public List<string> requiredPermissions { get; set; }

        public FeaturedTog featuredToggles { get; set; }

        public class Dis
        {
            public string title { get; set; }
            public string description { get; set; }
            public string navmapThumbnail { get; set; }
            public string favicon { get; set; }
        }

        public class Con
        {
            public string name { get; set; }
            public string email { get; set; }
        }

        public class Sce
        {
            public List<string> parcels { get; set; }

            public string @base { get; set; }
        }

        public class Spawn
        {
            public string name { get; set; }
            public bool @default { get; set; }
            public Vector3Int position { get; set; }
            public Vector3Int cameraTarget { get; set; }
        }

        public class FeaturedTog
        {

        }

        public List<ParcelInformation> GetParcelInformation()
        {
            List<ParcelInformation> output = new List<ParcelInformation>();

            foreach (string decentralandParcel in scene.parcels)
            {
                output.Add(new ParcelInformation(decentralandParcel));
            }
            
            return output;
        }

        public class ParcelInformation
        {
            private int x;
            private int z;

            public ParcelInformation(String decentralandParcel)
            {
                String[] spearator = { "," };
                Int32 count = 2;

                // using the method
                String[] strlist = decentralandParcel.Split(spearator, count, StringSplitOptions.RemoveEmptyEntries);

                x = int.Parse(strlist[0], NumberStyles.HexNumber);
                z = int.Parse(strlist[1], NumberStyles.HexNumber);
            }

            public ParcelInformation(int x, int z)
            {
                this.x = x;
                this.z = z;
            }

            public int GetX()
            {
                return x;
            }

            public int GetZ()
            {
                return z;
            }
        }
    }

    /*
     *{
  "display": {
    "title": "DCL Scene",
    "description": "My new Decentraland project",
    "navmapThumbnail": "images/scene-thumbnail.png",
    "favicon": "favicon_asset"
  },
  "owner": "",
  "contact": {
    "name": "author-name",
    "email": ""
  },
  "main": "bin/game.js",
  "tags": [],
  "scene": {
    "parcels": ["0,0"],
    "base": "0,0"
  },
  "spawnPoints": [
    {
      "name": "spawn1",
      "default": true,
      "position": {
        "x": 0,
        "y": 0,
        "z": 0
      },
      "cameraTarget": {
        "x": 8,
        "y": 1,
        "z": 8
      }
    }
  ],
  "requiredPermissions": [],
  "featureToggles": {}
}
 
     */

    
}
