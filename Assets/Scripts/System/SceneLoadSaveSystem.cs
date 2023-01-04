using Assets.Scripts.SceneState;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.System
{
    internal class SceneLoadException : Exception
    {
        public SceneLoadException() : base("A problem occurred while loading a scene")
        {
        }

        public SceneLoadException(string message) : base(message)
        {
        }
    }

    public interface ISceneLoadSystem
    {
        void Load(SceneDirectoryState sceneDirectoryState);
    }

    public interface ISceneSaveSystem
    {
        void Save(SceneDirectoryState sceneDirectoryState);
    }

    public class SceneLoadSaveSystem : ISceneLoadSystem, ISceneSaveSystem
    {
        // Dependencies
        private IPathState _pathState;

        public SceneLoadSaveSystem(IPathState pathState)
        {
            _pathState = pathState;
        }

        public void Save(SceneDirectoryState sceneDirectoryState)
        {
            DclSceneData sceneData = new DclSceneData(sceneDirectoryState.currentScene);
            string sceneDirPath = sceneDirectoryState.directoryPath;

            // Create scene directory in case it does not exist
            DirectoryInfo sceneDir = Directory.CreateDirectory(sceneDirPath);

            // Clear scene directory from files, that are regenerated
            foreach (FileInfo file in sceneDir
                         .GetFiles()
                         .Where(f => sceneDirectoryState.loadedFilePathsInScene.Contains(NormalizePath(f.FullName))))
                // Only delete files that were loaded into the scene.
                // This prevents the deletion of faulty entity files and files the user added manually into the scene folder
            {
                file.Delete();
            }

            sceneDirectoryState.loadedFilePathsInScene.Clear();

            // Create scene metadata file
            string metadataFilePath = $"{sceneDirPath}/scene.json";
            File.WriteAllText(metadataFilePath, "");

            sceneDirectoryState.loadedFilePathsInScene.Add(NormalizePath(metadataFilePath));

            // Create entity files
            foreach (var entity in sceneDirectoryState.currentScene!.AllEntities.Select(pair => pair.Value))
            {
                var newPath = CreateEntityFile(entity, sceneDirPath);

                sceneDirectoryState.loadedFilePathsInScene.Add(NormalizePath(newPath));
            }
        }

        public void Load(SceneDirectoryState sceneDirectoryState)
        {
            var scenePath = sceneDirectoryState.directoryPath;

            // return if path isn't directory
            if (!Directory.Exists(scenePath))
            {
                Debug.LogError("trying to load non existing scene");
                return;
            }

            // path should end with ".dclscene"
            if (!scenePath.EndsWith(".dclscene") && !scenePath.EndsWith(".dclscene/"))
            {
                Debug.LogWarning("scene folders should end with \".dclscene\"");
            }

            var scene = new DclScene();

            var directoryInfo = new DirectoryInfo(scenePath);
            foreach (var fileName in directoryInfo.GetFiles().Select(fi => fi.Name))
            {
                if (!fileName.EndsWith(".json"))
                {
                    continue;
                }

                if (fileName == "scene.json")
                {
                    sceneDirectoryState.loadedFilePathsInScene.Add("scene.json");
                    continue;
                }

                try
                {
                    LoadEntityFile(scene, directoryInfo.FullName + "/" + fileName);

                    sceneDirectoryState.loadedFilePathsInScene.Add(fileName);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            sceneDirectoryState.currentScene = scene;
        }

        /**
         * <summary>
         * Creates a save file for a given entity
         * </summary>
         * <param name="entity">The entity to save</param>
         * <param name="sceneDirectoryPath">The directory where the entity should be saved to</param>
         * <returns>
         * The path to the newly created file
         * </returns>
         */
        public string CreateEntityFile(DclEntity entity, string sceneDirectoryPath)
        {
            DclEntityData data = new DclEntityData(entity);
            string dataJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            string filename = data.customName.Replace(' ', '_') + "-" + data.guid.ToString() + ".json";

            var path = $"{sceneDirectoryPath}/{filename}";
            File.WriteAllText(path, dataJson);

            return path;
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
            public string customName;
            public Guid guid;
            public Guid? parentGuid;
            public bool isExposed;
            public List<DclComponentData> components;

            public DclEntityData(DclEntity entity)
            {
                this.customName = entity.CustomName;
                this.guid = entity.Id;
                this.parentGuid = entity.Parent?.Id;
                isExposed = entity.IsExposed;

                this.components = new List<DclComponentData>();
                foreach (DclComponent component in entity.Components)
                {
                    this.components.Add(new DclComponentData(component));
                }
            }

            public void MakeEntity(DclScene scene)
            {
                // Check for proper values
                if (customName == null)
                {
                    throw new SceneLoadException("Custom name was not set");
                }

                if (guid == default)
                {
                    throw new SceneLoadException($"Guid was not set for entity {customName}");
                }


                var dclEntity = new DclEntity(guid, customName, parentGuid ?? default, isExposed);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator 
                foreach (var component in components)
                {
                    var dclComponent = component.GetComponent();
                    if (dclComponent != null)
                    {
                        dclEntity.AddComponent(dclComponent);
                    }
                }

                scene.AddEntity(dclEntity);
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
                // Validate data
                if (string.IsNullOrEmpty(nameInCode))
                {
                    throw new SceneLoadException("Component has no nameInCode defined");
                }

                if (string.IsNullOrEmpty(nameOfSlot))
                {
                    throw new SceneLoadException("Component has no nameOfSlot defined");
                }

                var component = new DclComponent(nameInCode, nameOfSlot);

                foreach (var property in properties)
                {
                    var dclComponentProperty = property.GetProperty();

                    component.Properties.Add(dclComponentProperty);
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
                // Validate data
                if (string.IsNullOrEmpty(name))
                {
                    throw new SceneLoadException("Property has no name defined");
                }

                if (string.IsNullOrEmpty(type))
                {
                    throw new SceneLoadException("Property has no type defined");
                }

                if (!Enum.TryParse(type, true, out DclComponent.DclComponentProperty.PropertyType typeEnum))
                {
                    throw new SceneLoadException($"Type {type} does not exist");
                }


                switch (typeEnum)
                {
                    case DclComponent.DclComponentProperty.PropertyType.None:
                        throw new SceneLoadException("Can't load property from type none");
                    case DclComponent.DclComponentProperty.PropertyType.String:
                        if (fixedValue.Type != JTokenType.String)
                        {
                            throw new SceneLoadException("String property has no String value");
                        }

                        return new DclComponent.DclComponentProperty<string>(name, fixedValue.ToObject<string>());
                    case DclComponent.DclComponentProperty.PropertyType.Int:
                        if (fixedValue.Type != JTokenType.Integer)
                        {
                            throw new SceneLoadException("Int property has no Integer value");
                        }

                        return new DclComponent.DclComponentProperty<int>(name, fixedValue.ToObject<int>());
                    case DclComponent.DclComponentProperty.PropertyType.Float:
                        if (fixedValue.Type != JTokenType.Float)
                        {
                            throw new SceneLoadException("Float property has no Float value");
                        }

                        return new DclComponent.DclComponentProperty<float>(name, fixedValue.ToObject<float>());
                    case DclComponent.DclComponentProperty.PropertyType.Boolean:
                        if (fixedValue.Type != JTokenType.Boolean)
                        {
                            throw new SceneLoadException("Boolean property has no Boolean value");
                        }

                        return new DclComponent.DclComponentProperty<bool>(name, fixedValue.ToObject<bool>());
                    case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        if (fixedValue.Type != JTokenType.Object)
                        {
                            throw new SceneLoadException("Vector3 property has no Object value");
                        }

                        try
                        {
                            fixedValue.SelectToken("x", true);
                            fixedValue.SelectToken("y", true);
                            fixedValue.SelectToken("z", true);
                        }
                        catch (Exception)
                        {
                            throw new SceneLoadException("Vector3 property does not contain x, y, and z values");
                        }

                        return new DclComponent.DclComponentProperty<Vector3>(name, fixedValue.ToObject<Vector3>());
                    case DclComponent.DclComponentProperty.PropertyType.Quaternion:
                        if (fixedValue.Type != JTokenType.Object)
                        {
                            throw new SceneLoadException("Quaternion property has no Object value");
                        }

                        try
                        {
                            fixedValue.SelectToken("x", true);
                            fixedValue.SelectToken("y", true);
                            fixedValue.SelectToken("z", true);
                            fixedValue.SelectToken("w", true);
                        }
                        catch (Exception)
                        {
                            throw new SceneLoadException("Quaternion property does not contain x, y, z, and w values");
                        }

                        return new DclComponent.DclComponentProperty<Quaternion>(name, fixedValue.ToObject<Quaternion>());
                    case DclComponent.DclComponentProperty.PropertyType.Asset:
                        if (fixedValue.Type != JTokenType.String)
                        {
                            throw new SceneLoadException("Asset property has no String value");
                        }

                        return new DclComponent.DclComponentProperty<Guid>(name, fixedValue.ToObject<Guid>());
                    default:
                        throw new SceneLoadException("Unknown property type");
                }
            }
        }

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
        }
    }
}
