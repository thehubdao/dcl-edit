using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.EditorState;
using Assets.Scripts.ProjectState;
using Assets.Scripts.SceneState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class LoadFromVersion1System : MonoBehaviour
    {
        public void Load()
        {
            var sceneSaveFilePath = "";
            var projectSaveFilePath = "";
            var assetSaveFilePath = "";
            if (File.Exists(EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/save.json"))
            {
                sceneSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/save.json";
                projectSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/project.json";
                assetSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/assets.json";
            }
            else if (File.Exists(EditorStates.CurrentPathState.ProjectPath + "/scene/scene.json"))
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
                var newEntity = new DclEntity(newScene, Guid.NewGuid(), entity.name);


                //newEntity.IsExposed = entity.exposed;

                //newEntity.HierarchyOrder = entity.hierarchyOrder;
                //newEntity.CollapsedChildren = entity.collapsedChildren;

                parentNumbers.Add(newEntity, entity.parent);
                uniqueNumbers.Add(entity.uniqueNumber, newEntity);

                foreach (var component in entity.components)
                {
                    switch (component.name)
                    {
                        case "transform":
                            newEntity.Components.Add(MakeTransformComponentFromJson(component));
                            break;

                        case "GLTFShape":
                            newEntity.Components.Add(MakeGLTFShapeComponentFromJson(component, assetsJson));
                            break;

                        case "boxShape":
                            newEntity.Components.Add(MakeBoxShapeComponentFromJson(component));
                            break;

                        case "coneShape":
                            newEntity.Components.Add(MakeConeShapeComponentFromJson(component));
                            break;

                        case "cylinderShape":
                            newEntity.Components.Add(MakeCylinderShapeComponentFromJson(component));
                            break;

                        case "sphereShape":
                            newEntity.Components.Add(MakeSphereShapeComponentFromJson(component));
                            break;

                        case "planeShape":
                            newEntity.Components.Add(MakePlaneShapeComponentFromJson(component));
                            break;

                        default:
                            Debug.Log($"Component {component.name} not supported yet");
                            break;
                    }



                    //var newComponent =
                    //    newEntityGameObject
                    //            .AddComponent(ComponentRepresentationList.GetComponentByName(component.name))
                    //        as EntityComponent;
                    //
                    //newComponent?.ApplySpecificJson(component.specifics);
                }


            }

            foreach (var entity in parentNumbers)
            {
                //entity.SetParentKeepLocalScale(uniqueNumbers.TryGetValue(parentNumbers[entity], out var e)
                //    ? (SceneTreeObject)e
                //    : (SceneTreeObject)DclSceneManager.SceneRoot);

                // every entity with a parent
                if (entity.Value >= 0)
                {
                    // set the parent
                    entity.Key.Parent = uniqueNumbers[entity.Value];
                }
            }


            EditorStates.CurrentSceneState.CurrentScene = newScene;
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

                EditorStates.CurrentProjectState.Assets.UsedAssets = usedAssets;

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

        private static DclComponent MakeTransformComponentFromJson(EntityComponentJson componentJson)
        {
            var specificTransformJson = JsonUtility.FromJson<SpecificTransformJson>(componentJson.specifics);

            var newTransformComponent = new DclComponent("transform", "transform");
            newTransformComponent.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("position", specificTransformJson.pos));
            newTransformComponent.Properties.Add(new DclComponent.DclComponentProperty<Quaternion>("rotation", specificTransformJson.rot));
            newTransformComponent.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("scale", specificTransformJson.scale));

            return newTransformComponent;
        }

        private struct SpecificGLTFShapeJson
        {
            public string assetID;
        }

        private static DclComponent MakeGLTFShapeComponentFromJson(EntityComponentJson componentJson, AssetsJsonWrapper assetsJson)
        {
            var specificTransformJson = JsonUtility.FromJson<SpecificGLTFShapeJson>(componentJson.specifics);

            var assetJson = assetsJson.gltfAssets.Find(a => a.id == specificTransformJson.assetID);

            var newGltfShapeComponent = new DclComponent("GLTFShape", "Shape");
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<Guid>("asset", Guid.Parse(specificTransformJson.assetID)));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("visible", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("withCollisions", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("isPointerBlocker", true));

            return newGltfShapeComponent;
        }


        private static DclComponent MakeBoxShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("BoxShape", "Shape"); ;
        }

        private static DclComponent MakeSphereShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("SphereShape", "Shape");
        }

        private static DclComponent MakeCylinderShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("CylinderShape", "Shape");
        }

        private static DclComponent MakePlaneShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("PlaneShape", "Shape");
        }

        private static DclComponent MakeConeShapeComponentFromJson(EntityComponentJson _)
        {
            return new DclComponent("ConeShape", "Shape");
        }
    }

    [Serializable]
    public class EntityJson
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
    public class EntityComponentJson
    {
        //public EntityComponentJson(EntityComponent ec)
        //{
        //    name = ec.ComponentName;
        //    specifics = ec.SpecificJson;
        //}

        public string name;
        public string specifics;
    }

    public static class SceneSaveJsonHelper
    {
        //public static T[] FromJson<T>(string json)
        //{
        //    return wrapper.Items;
        //}

        [Serializable]
        public class EntityList
        {
            public int entityNumberCounter;
            public List<EntityJson> entities;// = new List<Entity.Json>();
        };

        public static EntityList FromJson(this string jsonString)
        {
            return JsonUtility.FromJson<EntityList>(jsonString);
        }

        public static string Indent(this String s)
        {
            return "    " + s.Replace("\n", "\n    ");
        }
    }
}
