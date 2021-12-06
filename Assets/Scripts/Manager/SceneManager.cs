using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class SceneManager : Manager, ISerializedFieldToStatic
{
    // Project Path
    public static string DclProjectPath = "";

    //void Start()
    //{
    //    _entityGameObject = entityGameObject;
    //    _entityTemplate = entityTemplate;
    //    GizmoCamera = _gizmoCamera;
    //}

    public void SetupStatics()
    {
        Debug.Log("init scene manager");
        _entityGameObject = entityGameObject;
        _entityTemplate = entityTemplate;
        GizmoCamera = _gizmoCamera;
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
    //public static float GizmoScale;
    //[SerializeField]
    //private float _gizmoScale;


    // Entities
    public GameObject entityGameObject;
    private static GameObject _entityGameObject;
    public static Entity[] Entities => _entityGameObject.GetComponentsInChildren<Entity>().Where(entity => !entity.doomed).ToArray();
    public static Transform EntityParent => _entityGameObject.transform;

    public GameObject entityTemplate;
    private static GameObject _entityTemplate;
    public static GameObject EntityTemplate => _entityTemplate;

    // Hierarchy View
    public static UnityEvent OnUpdateHierarchy = new UnityEvent();
    //public HierarchyView hierarchyView;
    //private static HierarchyView _hierarchyView;
    public static void ChangedHierarchy()
    {
        //_hierarchyView.UpdateVisuals();
        OnUpdateHierarchy.Invoke();
    }


    // Selection
    public static UnityEvent OnUpdateSelection = new UnityEvent();
    private static Entity _primarySelectedEntity;

    public static Entity PrimarySelectedEntity
    {
        get => _primarySelectedEntity;
        private set
        {
            _primarySelectedEntity = value;

            //foreach (var entity in Entities)
            //{
            //    var isSelected = entity == _primarySelectedEntity;
            //    entity.gizmos.SetActive(isSelected);
            //}

            OnUpdateSelection.Invoke();
        }
    }

    private static List<Entity> _secondarySelectedEntity = new List<Entity>();
    public static IEnumerable<Entity> SecondarySelectedEntity => _secondarySelectedEntity.AsEnumerable();

    public static void SetSelection(Entity entity)
    {
        var currentSecondarySelection = SecondarySelectedEntity.ToList();
        var currentPrimarySelection = PrimarySelectedEntity;

        UndoManager.RecordUndoItem(
            "Selected " + entity.TryGetShownName(),
            () =>
            {
                _secondarySelectedEntity = currentSecondarySelection;
                PrimarySelectedEntity = currentPrimarySelection;
            },
            () =>
            {
                _secondarySelectedEntity.Clear();
                PrimarySelectedEntity = entity;
            });


        SceneManager._secondarySelectedEntity.Clear();
        SceneManager.PrimarySelectedEntity = entity;
    }

    public static void AddSelection(Entity entity)
    {
        if (entity == null)
            return;

        var beforeSecondarySelection = SecondarySelectedEntity.ToList();
        var beforePrimarySelection = PrimarySelectedEntity;


        SceneManager._secondarySelectedEntity.Add(SceneManager.PrimarySelectedEntity);

        if(SceneManager._secondarySelectedEntity.Contains(entity))
            SceneManager._secondarySelectedEntity.Remove(entity);

        SceneManager.PrimarySelectedEntity = entity;
        
        var afterSecondarySelection = SecondarySelectedEntity.ToList();
        var afterPrimarySelection = PrimarySelectedEntity;

        UndoManager.RecordUndoItem(
            "Selected " + entity.ShownName,
            () =>
            {
                _secondarySelectedEntity = beforeSecondarySelection;
                PrimarySelectedEntity = beforePrimarySelection;
            },
            () =>
            {
                _secondarySelectedEntity = afterSecondarySelection;
                PrimarySelectedEntity = afterPrimarySelection;
            });
        
    }

    public static IEnumerable<Entity> AllSelectedEntities => _secondarySelectedEntity.Append(PrimarySelectedEntity);
    

    public static UnityEvent OnSelectedEntityTransformChange = new UnityEvent();

}

public class EntityArray : List<Entity>
{
    public EntityArray(Entity[] array) : base(array)
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
