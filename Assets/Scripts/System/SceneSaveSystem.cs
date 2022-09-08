using Assets.Scripts.SceneState;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Assets.Scripts.EditorState;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.System {
    public class SceneSaveSystem
    {
        public static void Save(DclScene scene)
        {
            DclSceneData sceneData = new DclSceneData(scene);

            // Create scene directory (if not exists)
            string sceneDirPath = $"{EditorStates.CurrentPathState.ProjectPath}/dcl-edit/saves/v2/{sceneData.name}.dclscene";
            DirectoryInfo sceneDir = Directory.CreateDirectory(sceneDirPath);

            // Clear scene directory
            foreach (FileInfo file in sceneDir.GetFiles()) file.Delete();

            // Create scene metadata file
            string metadataFilePath = $"{sceneDirPath}/scene.json";
            File.WriteAllText(metadataFilePath, "");

            // Create entity files
            foreach (var entity in scene.AllEntities)
            {
                CreateEntityFile(entity, sceneDirPath);
            }
        }


        public static void CreateEntityFile(KeyValuePair<Guid, DclEntity> entity, string sceneDirectoryPath)
        {
            DclEntityData data = new DclEntityData(entity.Value);
            string dataJson = JsonConvert.SerializeObject(data);

            string filename = data.customName.Replace(' ', '_') + "-" + data.guid.ToString() + ".json";

            File.WriteAllText($"{sceneDirectoryPath}/{filename}", dataJson);
        }


        public struct DclSceneData
        {
            public string name;

            public DclSceneData(DclScene scene)
            {
                this.name = scene.name;
            }
        }
        public struct DclEntityData
        {
            public string shownName;
            public string customName;
            public Guid guid;
            public Guid? parentGuid;
            public List<DclComponentData> components;

            public DclEntityData(DclEntity entity)
            {
                this.shownName = entity.ShownName;
                this.customName = entity.CustomName;
                this.guid = entity.Id;
                this.parentGuid = entity.Parent?.Id;

                this.components = new List<DclComponentData>();
                foreach (DclComponent component in entity.Components)
                {
                    this.components.Add(new DclComponentData(component));
                }
            }
        }
        public struct DclComponentData
        {
            public string nameInCode;
            public string nameOfSlot;
            public List<DclComponentPropertyData> properties;

            public DclComponentData(DclComponent component)
            {
                this.nameInCode = component.NameInCode;
                this.nameOfSlot = component.NameOfSlot;
                this.properties = new List<DclComponentPropertyData>();
                foreach (DclComponent.DclComponentProperty property in component.Properties)
                {
                    this.properties.Add(new DclComponentPropertyData(property));
                }
            }
        }
        public struct DclComponentPropertyData
        {
            public string name;
            public string type;
            public JToken fixedValue;

            public DclComponentPropertyData(DclComponent.DclComponentProperty property)
            {
                this.name = property.PropertyName;
                this.type = property.Type.ToString();
                this.fixedValue = null;
                switch (property.Type)
                {
                    case DclComponent.DclComponentProperty.PropertyType.None:
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.String:
                        this.fixedValue = JToken.FromObject(property.GetConcrete<string>().FixedValue);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Int:
                        this.fixedValue = JToken.FromObject(property.GetConcrete<int>().FixedValue);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Float:
                        this.fixedValue = JToken.FromObject(property.GetConcrete<float>().FixedValue);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Boolean:
                        this.fixedValue = JToken.FromObject(property.GetConcrete<bool>().FixedValue);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        this.fixedValue = JToken.FromObject(property.GetConcrete<Vector3>().FixedValue);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Quaternion:
                        this.fixedValue = JToken.FromObject(property.GetConcrete<Quaternion>().FixedValue);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Asset:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}