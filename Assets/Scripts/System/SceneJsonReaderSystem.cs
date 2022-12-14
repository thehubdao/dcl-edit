using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Assets.Scripts.EditorState;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class SceneJsonReaderSystem
    {
        private static DecentralandSceneData decentralandSceneData;

        // Dependencies
        private IPathState pathState;

        [Inject]
        private void Construct(IPathState pathState)
        {
            this.pathState = pathState;
        }

        /// <summary>
        /// Loads the Scene Data that the user works on. Loads it just once and stores it as a static variable
        /// </summary>
        /// <param name="reload">if it should reload the scene data from the json file again</param>
        /// <returns>the Decentraland scene data; if there is an error it returns null</returns>
        [CanBeNull]
        public DecentralandSceneData GetSceneData(bool reload)
        {
            try
            {
                if (reload || decentralandSceneData == null)
                {
                    var json = File.ReadAllText(pathState.ProjectPath + "/scene.json");
                    decentralandSceneData = JsonConvert.DeserializeObject<DecentralandSceneData>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error while reading scene.json");
                Debug.LogException(e);
                return null;
            }

            return decentralandSceneData;
        }

        /// <summary>
        /// Stores the Decentraland Scene Data
        /// </summary>
        public class DecentralandSceneData
        {
            public Display display { get; set; }
            public string owner { get; set; }
            public Contact contact { get; set; }

            public string main { get; set; }

            public List<string> tags { get; set; }

            public Scene scene { get; set; }

            public List<SpawnPoints> spawnPoints { get; set; }

            public List<string> requiredPermissions { get; set; }

            public FeatureToggles featureToggles { get; set; }

            public class Display
            {
                public string title { get; set; }
                public string description { get; set; }
                public string navmapThumbnail { get; set; }
                public string favicon { get; set; }
            }

            public class Contact
            {
                public string name { get; set; }
                public string email { get; set; }
            }

            public class Scene
            {
                public List<string> parcels { get; set; }

                public string @base { get; set; }
            }

            public class SpawnPoints
            {
                public string name { get; set; }
                public bool @default { get; set; }
                public Vector3Int position { get; set; }
                public Vector3Int cameraTarget { get; set; }
            }

            public class FeatureToggles
            {
            }

            public List<ParcelInformation> GetParcelsInformation()
            {
                List<ParcelInformation> output = new List<ParcelInformation>();

                foreach (string decentralandParcel in scene.parcels)
                {
                    output.Add(new ParcelInformation(decentralandParcel));
                }

                return output;
            }

            public ParcelInformation GetBaseParcelInformation()
            {
                return new ParcelInformation(scene.@base);
            }

            public class ParcelInformation
            {
                private Vector2Int position;

                public ParcelInformation(String decentralandParcel)
                {
                    String[] spearator = {","};
                    Int32 count = 2;

                    // using the method
                    String[] strlist = decentralandParcel.Split(spearator, count, StringSplitOptions.RemoveEmptyEntries);

                    var x = int.Parse(strlist[0], NumberStyles.HexNumber);
                    var z = int.Parse(strlist[1], NumberStyles.HexNumber);

                    position = new Vector2Int(x, z);
                }

                public ParcelInformation(int x, int z)
                {
                    position = new Vector2Int(x, z);
                }

                public Vector2Int GetPosition() => position;
            }
        }

        /*
        * {
        *   "display": {
        *     "title": "DCL Scene",
        *     "description": "My new Decentraland project",
        *     "navmapThumbnail": "images/scene-thumbnail.png",
        *     "favicon": "favicon_asset"
        *   },
        *   "owner": "",
        *   "contact": {
        *     "name": "author-name",
        *     "email": ""
        *   },
        *   "main": "bin/game.js",
        *   "tags": [],
        *   "scene": {
        *     "parcels": ["0,0"],
        *     "base": "0,0"
        *   },
        *   "spawnPoints": [
        *     {
        *       "name": "spawn1",
        *       "default": true,
        *       "position": {
        *         "x": 0,
        *         "y": 0,
        *         "z": 0
        *       },
        *       "cameraTarget": {
        *         "x": 8,
        *         "y": 1,
        *         "z": 8
        *       }
        *     }
        *   ],
        *   "requiredPermissions": [],
        *   "featureToggles": {}
        * }  
        */
    }
}
