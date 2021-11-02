using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
//using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoBehaviour
{
    // Constants 
    public static readonly string DclProjectPath = "F:/Data/Decentraland/dcl-edit-test";

    // Start is called before the first frame update
    void Start()
    {
        var sr = new StreamReader(DclProjectPath+"/scene.json");
        var fileContents = sr.ReadToEnd();
        sr.Close();

        sceneJson = JsonUtility.FromJson<SceneJson>(fileContents);

        _saveManager = saveManager;

        _entityGameObject = entityGameObject;
        _entityTemplate = entityTemplate;
        //_hierarchyView = hierarchyView;
        _gizmoRelationManager = gizmoRelationManager;
        ChangedHierarchy();

        SelectedEntity = null;

        GizmoCamera = _gizmoCamera;
        GizmoScale = _gizmoScale;

        SaveManager.Load(); 
    }


    // read scene.json file 
    public struct SceneJson
    {
        [Serializable]
        public struct Display
        {
            public string title;
            public string description;
            public string navmapThumbnail;
            public string favicon;
        }
        public Display display;
        
        [Serializable]
        public struct Contact
        {
            public string name;
            public string email;
        }
        public Contact contact;

        public string owner;

        [Serializable]
        public struct Scene
        {
            [SerializeField]
            private string[] parcels;
            [SerializeField]
            private string @base;

            public Vector2Int[] Parcels
            {
                get
                {
                    Vector2Int[] retval = new Vector2Int[parcels.Length];
                    for (var index = 0; index < parcels.Length; index++)
                    {
                        var parcel = parcels[index];
                        retval[index] = parcel.ToVec2Int();
                    }

                    return retval;
                }
            }

            public Vector2Int Base => @base.ToVec2Int();
        }
        public Scene scene;


    }
    public static SceneJson sceneJson;


    // Gizmo sizing 
    public static Camera GizmoCamera;
    [SerializeField]
    private Camera _gizmoCamera;
    public static float GizmoScale;
    [SerializeField]
    private float _gizmoScale;
    
    // Saving and Loading
    public SceneSaveManager saveManager;
    private static SceneSaveManager _saveManager;
    public static SceneSaveManager SaveManager => _saveManager;

    // Entities
    public GameObject entityGameObject;
    private static GameObject _entityGameObject;
    public static Entity[] Entities => _entityGameObject.GetComponentsInChildren<Entity>().Where(entity => !entity.doomed).ToArray();
    public static Transform EntityParent => _entityGameObject.transform;
    
    public GameObject entityTemplate;
    private static GameObject _entityTemplate;
    public static GameObject EntityTemplate => _entityTemplate;

    // Manipulator manager
    public GizmoRelationManager gizmoRelationManager;
    private static GizmoRelationManager _gizmoRelationManager;
    public static GizmoRelationManager GizmoRelationManager => _gizmoRelationManager;

    // Hierarchy View
    public static UnityEvent OnUpdateHierarchy = new UnityEvent();
    //public HierarchyView hierarchyView;
    //private static HierarchyView _hierarchyView;
    public static void ChangedHierarchy()
    {
        //_hierarchyView.UpdateVisuals();
        OnUpdateHierarchy.Invoke();
    }

    // Inspector


    // Selection
    public static UnityEvent OnUpdateSelection = new UnityEvent();
    private static Entity _selectedEntity;
    public static Entity SelectedEntity
    {
        get => _selectedEntity;
        set
        {
            _selectedEntity = value;

            foreach (var entity in Entities)
            {
                var isSelected = entity == _selectedEntity;
                entity.gizmos.SetActive(isSelected);
            }

            // TODO update inspector
            OnUpdateSelection.Invoke();
        }
    }
}

public class EntityArray:List<Entity>
{
    public EntityArray(Entity[] array):base(array)
    {
    }
}

public static class MyJsonUtil
{
    public static Vector2Int ToVec2Int(this string json)
    {
        var parts = json.Split(',');

        if (parts.Length != 2)
        {
            throw new Exception("trying to parse a non vec2 json field to Vector2Int");
        }

        return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
    }
}
