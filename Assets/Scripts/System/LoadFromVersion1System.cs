using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.EditorState;
using Assets.Scripts.State;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class LoadFromVersion1System : MonoBehaviour
    {
        public void Load()
        {
            var sceneSaveFilePath = "";
            var projectSaveFilePath = "";
            if (File.Exists(EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/save.json"))
            {
                sceneSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/save.json";
                projectSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/project.json";
            }
            else if (File.Exists(EditorStates.CurrentPathState.ProjectPath + "/scene/scene.json"))
            {
                sceneSaveFilePath = EditorStates.CurrentPathState.ProjectPath + "/scene/scene.json";
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
                            newEntity.Components.Add(MakeGLTFShapeComponentFromJson(component));
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

        private static DclComponent MakeGLTFShapeComponentFromJson(EntityComponentJson componentJson)
        {
            var specificTransformJson = JsonUtility.FromJson<SpecificGLTFShapeJson>(componentJson.specifics);

            var newGltfShapeComponent = new DclComponent("GLTFShape", "Shape");
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<Guid>("asset",Guid.Parse(specificTransformJson.assetID)));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("visible",true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("withCollisions", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("isPointerBlocker", true));

            return newGltfShapeComponent;
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
