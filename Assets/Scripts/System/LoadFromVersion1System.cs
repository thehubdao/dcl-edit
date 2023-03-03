using Assets.Scripts.ProjectState;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class LoadFromVersion1System : ISceneLoadSystem
    {
        // Dependencies
        private EditorState.ProjectState _projectState;
        private IPathState _pathState;

        public LoadFromVersion1System(EditorState.ProjectState projectState, IPathState pathState)
        {
            _projectState = projectState;
            _pathState = pathState;
        }

        public void Load(SceneDirectoryState sceneDirectoryState)
        {
            var sceneSaveFilePath = "";
            var projectSaveFilePath = "";
            var assetSaveFilePath = "";
            if (File.Exists(_pathState.ProjectPath + "/dcl-edit/saves/save.json"))
            {
                sceneSaveFilePath = _pathState.ProjectPath + "/dcl-edit/saves/save.json";
                projectSaveFilePath = _pathState.ProjectPath + "/dcl-edit/saves/project.json";
                assetSaveFilePath = _pathState.ProjectPath + "/dcl-edit/saves/assets.json";
            }
            else if (File.Exists(_pathState.ProjectPath + "/scene/scene.json"))
            {
                return; // To old version. No support for this version
                //sceneSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/scene/scene.json";
            }

            if (projectSaveFilePath != "")
            {
                try
                {
                    //var projectJsonString = File.ReadAllText(projectSaveFilePath);
                    //ProjectData.ApplyJsonString(projectJsonString);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            // load assets
            var assetsJson = LoadAssets(assetSaveFilePath);

            // TODO: Check if there is all ready a scene loaded

            // create a new scene

            var newScene = new DclScene();

            var reader = new StreamReader(sceneSaveFilePath);
            var entities = reader.ReadToEnd().FromJson();
            reader.Close();

            //Entity.uniqueNumberCounter = entities.entityNumberCounter;

            //foreach (var entity in DclSceneManager.Entities)
            //{
            //    entity.doomed = true;
            //    Destroy(entity.gameObject);
            //}

            var parentNumbers = new Dictionary<DclEntity, int>();
            var uniqueNumbers = new Dictionary<int, DclEntity>();
            foreach (var entity in entities.entities)
            {
                var newEntity = new DclEntity(Guid.NewGuid(), entity.name, hierarchyOrder: entity.hierarchyOrder);


                //newEntity.IsExposed = entity.exposed;

                //newEntity.CollapsedChildren = entity.collapsedChildren;

                parentNumbers.Add(newEntity, entity.parent);
                uniqueNumbers.Add(entity.uniqueNumber, newEntity);

                foreach (var component in entity.components)
                {
                    switch (component.name)
                    {
                        case "transform":
                            newEntity.AddComponent(MakeTransformComponentFromJson(component));
                            break;

                        case "GLTFShape":
                            newEntity.AddComponent(MakeGLTFShapeComponentFromJson(component, assetsJson));
                            break;

                        case "boxShape":
                            newEntity.AddComponent(MakeBoxShapeComponentFromJson(component));
                            break;

                        case "coneShape":
                            newEntity.AddComponent(MakeConeShapeComponentFromJson(component));
                            break;

                        case "cylinderShape":
                            newEntity.AddComponent(MakeCylinderShapeComponentFromJson(component));
                            break;

                        case "sphereShape":
                            newEntity.AddComponent(MakeSphereShapeComponentFromJson(component));
                            break;

                        case "planeShape":
                            newEntity.AddComponent(MakePlaneShapeComponentFromJson(component));
                            break;

                        default:
                            Debug.Log($"Component {component.name} not supported yet");
                            break;
                    }
                }

                newScene.AddEntity(newEntity);
            }

            // set all the parents
            foreach (var entity in parentNumbers)
            {
                // every entity with a parent
                if (entity.Value >= 0)
                {
                    // set the parent
                    entity.Key.Parent = uniqueNumbers[entity.Value];
                }
            }

            // change entity position and rotation from global to local
            foreach (
                var entity in parentNumbers
                    .Where(p => p.Value < 0)
                    .Select(p => p.Key))
            {
                ChangeToLocal(entity, null);
            }

            sceneDirectoryState.currentScene = newScene;
        }

        public void LoadV1(SceneDirectoryState sceneDirectoryState) => Load(sceneDirectoryState);

        private void ChangeToLocal(DclEntity entity, DclTransformComponent dclParentTransform)
        {
            // can be null
            var dclTransform = entity.GetTransformComponent();

            foreach (var child in entity.Children)
            {
                ChangeToLocal(child, dclTransform);
            }

            if (dclTransform == null || dclParentTransform == null)
                return;

            // generate two game object to simplify the transformation maths
            var parentTransform = new GameObject().transform;
            var childTransform = new GameObject().transform;

            childTransform.SetParent(parentTransform);

            parentTransform.position = dclParentTransform.position.Value;
            parentTransform.rotation = dclParentTransform.rotation.Value;
            parentTransform.localScale = dclParentTransform.scale.Value;

            childTransform.position = dclTransform.position.Value;
            childTransform.rotation = dclTransform.rotation.Value;
            childTransform.localScale = dclTransform.scale.Value;

            dclTransform.position.SetFixedValue(childTransform.localPosition);
            dclTransform.rotation.SetFixedValue(childTransform.localRotation);

            // cleanup
            UnityEngine.Object.Destroy(parentTransform.gameObject);
            UnityEngine.Object.Destroy(childTransform.gameObject);
        }

        private AssetsJsonWrapper LoadAssets(string path)
        {
            if (File.Exists(path))
            {
                var fileContent = File.ReadAllText(path);
                var assetsJson = JsonUtility.FromJson<AssetsJsonWrapper>(fileContent);

                var usedAssets = new Dictionary<Guid, DclAsset>();

                foreach (var assetJson in assetsJson.gltfAssets)
                {
                    usedAssets.Add(Guid.Parse(assetJson.id), new DclGltfAsset(assetJson.name, assetJson.gltfPath));
                }

                _projectState.Assets.UsedAssets = usedAssets;

                return assetsJson;
            }

            throw new IOException($"File {path} not found");
        }

        [Serializable]
        private class AssetsJsonWrapper
        {
            [Serializable]
            public struct GltfAssetWrapper
            {
                public string name;
                public string id;
                public string gltfPath;
            }

            public List<GltfAssetWrapper> gltfAssets;
        }

        private struct SpecificTransformJson
        {
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scale;
        }

        private DclComponent MakeTransformComponentFromJson(EntityComponentJson componentJson)
        {
            var specificTransformJson = JsonUtility.FromJson<SpecificTransformJson>(componentJson.specifics);

            var newTransformComponent = new DclComponent("Transform", "Transform");
            newTransformComponent.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("position", specificTransformJson.pos)); // This will save the global position
            newTransformComponent.Properties.Add(new DclComponent.DclComponentProperty<Quaternion>("rotation", specificTransformJson.rot)); // This will save the global rotation
            newTransformComponent.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("scale", specificTransformJson.scale)); // This will save the local scale

            return newTransformComponent;
        }

        private struct SpecificGLTFShapeJson
        {
            public string assetID;
        }

        private DclComponent MakeGLTFShapeComponentFromJson(EntityComponentJson componentJson, AssetsJsonWrapper assetsJson)
        {
            var specificTransformJson = JsonUtility.FromJson<SpecificGLTFShapeJson>(componentJson.specifics);

            var assetJson = assetsJson.gltfAssets.Find(a => a.id == specificTransformJson.assetID);
            var (didParse, realId) = GetAssetIdFromAssetPath(assetJson.gltfPath);

            var newGltfShapeComponent = new DclComponent("GLTFShape", "Shape");
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<Guid>("asset", didParse ? realId : Guid.Parse(specificTransformJson.assetID)));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("visible", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("withCollisions", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("isPointerBlocker", true));

            return newGltfShapeComponent;
        }

        private (bool didParse, Guid assetId) GetAssetIdFromAssetPath(string assetJsonGltfPath)
        {
            var folderStructureArray = assetJsonGltfPath.Split('/');
            foreach (var folder in folderStructureArray)
            {
                if (Guid.TryParse(folder, out var realId))
                {
                    return (true, realId);
                }
            }
            
            return (false, Guid.Empty);
        }


        private DclComponent MakeBoxShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("BoxShape", "Shape");
        }

        private DclComponent MakeSphereShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("SphereShape", "Shape");
        }

        private DclComponent MakeCylinderShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("CylinderShape", "Shape");
        }

        private DclComponent MakePlaneShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("PlaneShape", "Shape");
        }

        private DclComponent MakeConeShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("ConeShape", "Shape");
        }
    }

    [Serializable]
    internal class EntityJson
    {
        //public EntityJson(Entity e)
        //{
        //    hierarchyOrder = e.HierarchyOrder;
        //    name = e.CustomName;
        //    uniqueNumber = e.uniqueNumber;
        //    parent = (e.Parent as Entity)?.uniqueNumber ?? -1;
        //    exposed = e.Exposed;
        //    collapsedChildren = e.CollapsedChildren;
        //    components = e.Components.Select(c => new EntityComponent.Json(c)).ToList();
        //}

        public float hierarchyOrder;
        public string name;
        public int uniqueNumber;
        public int parent;
        public bool exposed;
        public bool collapsedChildren;
        public List<EntityComponentJson> components;
    }

    [Serializable]
    internal class EntityComponentJson
    {
        //public EntityComponentJson(EntityComponent ec)
        //{
        //    name = ec.ComponentName;
        //    specifics = ec.SpecificJson;
        //}

        public string name;
        public string specifics;
    }

    internal static class SceneSaveJsonHelper
    {
        //public static T[] FromJson<T>(string json)
        //{
        //    return wrapper.Items;
        //}

        [Serializable]
        public class EntityList
        {
            public int entityNumberCounter;
            public List<EntityJson> entities; // = new List<Entity.Json>();
        };

        public static EntityList FromJson(this string jsonString)
        {
            return JsonUtility.FromJson<EntityList>(jsonString);
        }
    }
}