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
            Directory.CreateDirectory(DclSceneManager.DclProjectPath + "/dcl-edit/saves");
            try
            {
                var fileWriter = new StreamWriter(DclSceneManager.DclProjectPath + "/dcl-edit/saves/save.json", false);

                fileWriter.WriteLine(DclSceneManager.Entities.ToJson());

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
                var fileWriter = new StreamWriter(DclSceneManager.DclProjectPath + "/dcl-edit/saves/project.json", false);

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
        if (File.Exists(DclSceneManager.DclProjectPath + "/dcl-edit/saves/save.json"))
        {
            sceneSaveFilePath = DclSceneManager.DclProjectPath + "/dcl-edit/saves/save.json";
            projectSaveFilePath = DclSceneManager.DclProjectPath + "/dcl-edit/saves/project.json";
        }
        else if (File.Exists(DclSceneManager.DclProjectPath + "/scene/scene.json"))
        {
            sceneSaveFilePath = DclSceneManager.DclProjectPath + "/scene/scene.json";
        }

        if (projectSaveFilePath != "")
        {
            try
            {
                var projectJsonString = File.ReadAllText(projectSaveFilePath);
                ProjectData.ApplyJsonString(projectJsonString);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        if(sceneSaveFilePath != "")
        {
            var reader = new StreamReader(sceneSaveFilePath);
            var entities = reader.ReadToEnd().FromJson();
            reader.Close();

            Entity.uniqueNumberCounter = entities.entityNumberCounter;

            foreach (var entity in DclSceneManager.Entities)
            {
                entity.doomed = true;
                Destroy(entity.gameObject);
            }

            var parentNumbers = new Dictionary<Entity, Guid>();
            var uniqueNumbers = new Dictionary<Guid, Entity>();
            var oldUniqueValues = new Dictionary<int, Guid>();
            foreach (var entity in entities.entities)
            {
                var newEntityGameObject = Instantiate(DclSceneManager.EntityTemplate, DclSceneManager.EntityParent);
                var newEntity = newEntityGameObject.GetComponent<Entity>();
                var uniqueIdDidParse = Guid.TryParse(entity.uniqueId, out var uniqueId);
                var parentIdDidParse = Guid.TryParse(entity.parentId, out var parentId);

                if (!uniqueIdDidParse && !oldUniqueValues.ContainsKey(entity.uniqueNumber))
                {
                    oldUniqueValues.Add(entity.uniqueNumber, Guid.NewGuid());
                }

                if (!parentIdDidParse && !oldUniqueValues.ContainsKey(entity.parent))
                {
                    oldUniqueValues.Add(entity.parent, entity.parent == -1 ? default : Guid.NewGuid());
                }

                newEntity.HierarchyOrder = entity.hierarchyOrder;
                newEntity.CustomName = entity.name;
                newEntity.uniqueNumber = entity.uniqueNumber;
                newEntity.uniqueId = uniqueIdDidParse ? uniqueId : oldUniqueValues[entity.uniqueNumber];
                newEntity.Exposed = entity.exposed;
                newEntity.CollapsedChildren = entity.collapsedChildren;
                parentNumbers.Add(newEntity, parentIdDidParse ? parentId : oldUniqueValues[entity.parent]);
                uniqueNumbers.Add(uniqueIdDidParse ? uniqueId : oldUniqueValues[entity.uniqueNumber], newEntity);

                foreach (var component in entity.components)
                {
                    var newComponent = 
                        newEntityGameObject
                            .AddComponent(ComponentRepresentationList.GetComponentByName(component.name)) 
                            as EntityComponent;

                    newComponent?.ApplySpecificJson(component.specifics);
                }
            }

            foreach (var entity in DclSceneManager.Entities)
            {
                entity.SetParentKeepLocalScale(uniqueNumbers.TryGetValue(parentNumbers[entity], out var e)
                    ? (SceneTreeObject)e
                    : (SceneTreeObject)DclSceneManager.SceneRoot);
            }
            

            DclSceneManager.ChangedHierarchy();
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

