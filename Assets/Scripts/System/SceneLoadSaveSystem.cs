using Assets.Scripts.SceneState;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.System
{
    public interface ISceneLoadSystem
    {
        DclScene Load(string absolutePath);
    }

    public interface ISceneSaveSystem
    {
        void Save(DclScene scene);
    }

    public class SceneLoadSaveSystem : ISceneLoadSystem, ISceneSaveSystem
    {
        // Dependencies
        private PathState _pathState;

        public SceneLoadSaveSystem(PathState pathState)
        {
            _pathState = pathState;
        }

        public void Save(DclScene scene)
        {
            DclSceneData sceneData = new DclSceneData(scene);

            // Create scene directory (if not exists)
            string sceneDirPath = $"{_pathState.ProjectPath}/dcl-edit/saves/v2/{sceneData.name}.dclscene";
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

        public DclScene Load(string absolutePath)
        {
            // return if path isn't directory
            if (!Directory.Exists(absolutePath))
            {
                Debug.LogError("trying to load non existing scene");
                return null;
            }

            // path should end with ".dclscene"
            if (!absolutePath.EndsWith(".dclscene") && !absolutePath.EndsWith(".dclscene/"))
            {
                Debug.LogWarning("scene folders should end with \".dclscene\"");
            }

            var scene = new DclScene();

            var directoryInfo = new DirectoryInfo(absolutePath);
            foreach (var fileName in directoryInfo.GetFiles().Select(fi => fi.Name))
            {
                if (fileName == "scene.json" || !fileName.EndsWith(".json"))
                {
                    continue;
                }

                LoadEntityFile(scene, directoryInfo.FullName + "/" + fileName);
            }


            return scene;
        }


        public void CreateEntityFile(KeyValuePair<Guid, DclEntity> entity, string sceneDirectoryPath)
        {
            DclEntityData data = new DclEntityData(entity.Value);
            string dataJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            string filename = data.customName.Replace(' ', '_') + "-" + data.guid.ToString() + ".json";

            File.WriteAllText($"{sceneDirectoryPath}/{filename}", dataJson);
        }

        public void LoadEntityFile(DclScene scene, string absolutePath)
        {
            var json = File.ReadAllText(absolutePath);
            var entityData = JsonConvert.DeserializeObject<DclEntityData>(json);

            entityData.MakeEntity(scene);
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

            public void MakeEntity(DclScene scene)
            {
                var dclEntity = new DclEntity(scene, guid, customName, parentGuid ?? default);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator 
                foreach (var component in components)
                {
                    var dclComponent = component.GetComponent();
                    if (dclComponent != null)
                    {
                        dclEntity.AddComponent(dclComponent);
                    }
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

            public DclComponent GetComponent()
            {
                var component = new DclComponent(nameInCode, nameOfSlot);
                try
                {
                    foreach (var property in properties)
                    {
                        var dclComponentProperty = property.GetProperty();

                        if (dclComponentProperty == null)
                        {
                            return null;
                        }

                        component.Properties.Add(dclComponentProperty);
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                return component;
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
                        this.fixedValue = JToken.FromObject(property.GetConcrete<Guid>().FixedValue);
                        break;
                    default:
                        break;
                }
            }

            public DclComponent.DclComponentProperty GetProperty()
            {
                if (!Enum.TryParse(type, true, out DclComponent.DclComponentProperty.PropertyType typeEnum))
                {
                    Debug.LogError($"Type {type} does not exist");
                    return null;
                }

                switch (typeEnum)
                {
                    case DclComponent.DclComponentProperty.PropertyType.None:
                        return null;
                    case DclComponent.DclComponentProperty.PropertyType.String:
                        return new DclComponent.DclComponentProperty<string>(name, fixedValue.ToObject<string>());
                    case DclComponent.DclComponentProperty.PropertyType.Int:
                        return new DclComponent.DclComponentProperty<int>(name, fixedValue.ToObject<int>());
                    case DclComponent.DclComponentProperty.PropertyType.Float:
                        return new DclComponent.DclComponentProperty<float>(name, fixedValue.ToObject<float>());
                    case DclComponent.DclComponentProperty.PropertyType.Boolean:
                        return new DclComponent.DclComponentProperty<bool>(name, fixedValue.ToObject<bool>());
                    case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        return new DclComponent.DclComponentProperty<Vector3>(name, fixedValue.ToObject<Vector3>());
                    case DclComponent.DclComponentProperty.PropertyType.Quaternion:
                        return new DclComponent.DclComponentProperty<Quaternion>(name, fixedValue.ToObject<Quaternion>());
                    case DclComponent.DclComponentProperty.PropertyType.Asset:
                        return new DclComponent.DclComponentProperty<Guid>(name, fixedValue.ToObject<Guid>());
                    default:
                        return null;
                }
            }
        }
    }
}
