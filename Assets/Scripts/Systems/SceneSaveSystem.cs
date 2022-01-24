using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SceneSaveSystem : MonoBehaviour
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

    public static void Save()
    {
        NormalizeHierarchyOrderValues.Normalize();
        
        try
        {
            Directory.CreateDirectory(SceneManager.DclProjectPath + "/dcl-edit/saves");
            try
            {
                var fileWriter = new StreamWriter(SceneManager.DclProjectPath + "/dcl-edit/saves/save.json", false);

                fileWriter.WriteLine(SceneManager.Entities.ToJson());

                fileWriter.Close();

                HoverLabelManager.OpenLabel("Scene Saved");
            }
            catch (IOException)
            {
                Debug.LogError("Error while saving scene");
                HoverLabelManager.OpenLabel("Error while saving scene");
            }

            try
            {
                var fileWriter = new StreamWriter(SceneManager.DclProjectPath + "/dcl-edit/saves/project.json", false);

                fileWriter.WriteLine(ProjectData.ToJson());

                fileWriter.Close();

                HoverLabelManager.OpenLabel("Scene Saved");
            }
            catch (IOException)
            {
                Debug.LogError("Error while saving scene");
                HoverLabelManager.OpenLabel("Error while saving scene");
            }

        }
        catch (IOException)
        {
            Debug.LogError("Error while saving scene");
            HoverLabelManager.OpenLabel("Error while saving scene");
        }
    }

    public static void Load()
    {
        var sceneSaveFilePath = "";
        var projectSaveFilePath = "";
        if (File.Exists(SceneManager.DclProjectPath + "/dcl-edit/saves/save.json"))
        {
            sceneSaveFilePath = SceneManager.DclProjectPath + "/dcl-edit/saves/save.json";
            projectSaveFilePath = SceneManager.DclProjectPath + "/dcl-edit/saves/project.json";
        }
        else if (File.Exists(SceneManager.DclProjectPath + "/scene/scene.json"))
        {
            sceneSaveFilePath = SceneManager.DclProjectPath + "/scene/scene.json";
        }

        if (projectSaveFilePath != "")
        {
            var projectJsonString = File.ReadAllText(projectSaveFilePath);
            ProjectData.ApplyJsonString(projectJsonString);
        }
        
        if(sceneSaveFilePath != "")
        {
            var reader = new StreamReader(sceneSaveFilePath);
            var entities = reader.ReadToEnd().FromJson();
            reader.Close();

            Entity.uniqueNumberCounter = entities.entityNumberCounter;

            foreach (var entity in SceneManager.Entities)
            {
                entity.doomed = true;
                Destroy(entity.gameObject);
            }

            var parentNumbers = new Dictionary<Entity, int>();
            var uniqueNumbers = new Dictionary<int, Entity>();
            foreach (var entity in entities.entities)
            {
                var newEntityGameObject = Instantiate(SceneManager.EntityTemplate, SceneManager.EntityParent);
                var newEntity = newEntityGameObject.GetComponent<Entity>();

                newEntity.HierarchyOrder = entity.hierarchyOrder;
                newEntity.CustomName = entity.name;
                newEntity.uniqueNumber = entity.uniqueNumber;
                newEntity.Exposed = entity.exposed;
                newEntity.CollapsedChildren = entity.collapsedChildren;
                parentNumbers.Add(newEntity,entity.parent);
                uniqueNumbers.Add(entity.uniqueNumber, newEntity);

                foreach (var component in entity.components)
                {
                    EntityComponent newComponent = component.name switch
                    {
                        "transform" => newEntityGameObject.AddComponent<TransformComponent>(),
                        "sphereShape" => newEntityGameObject.AddComponent<SphereShapeComponent>(),
                        "boxShape" => newEntityGameObject.AddComponent<BoxShapeComponent>(),
                        "planeShape" => newEntityGameObject.AddComponent<PlaneShapeComponent>(),
                        "GLTFShape" => newEntityGameObject.AddComponent<GLTFShapeComponent>(),
                        _ => throw new NotImplementedException("Unknown component name: " + component.name)
                    };

                    newComponent.ApplySpecificJson(component.specifics);
                }
            }

            foreach (var entity in SceneManager.Entities)
            {
                entity.Parent = uniqueNumbers.TryGetValue(parentNumbers[entity], out var e)
                    ? (SceneTreeObject)e
                    : (SceneTreeObject)SceneManager.SceneRoot;
            }
            

            SceneManager.ChangedHierarchy();
        }
        else
        {
            Debug.Log("Creating new Scene...");
        }
    }
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
        public List<Entity.Json> entities;// = new List<Entity.Json>();
    };

    public static EntityList FromJson(this string jsonString)
    {
        return JsonUtility.FromJson<EntityList>(jsonString);
    }

    public static string ToJson(this Entity[] entities)
    {

        var entityList = new EntityList()
        {
            entities = entities.Select(e => new Entity.Json(e)).ToList(),
            entityNumberCounter = Entity.uniqueNumberCounter
        };

        return JsonUtility.ToJson(entityList,true);
    }

    public static string Indent(this String s)
    {
        return "    "+s.Replace("\n", "\n    ");
    }
    
}

