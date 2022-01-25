using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRepresentationList : MonoBehaviour, ISerializedFieldToStatic
{

    [Header("Scene Representations")]
    [SerializeField]
    private GameObject _transformComponentRepresentation = default;
    public static GameObject TransformComponentInScene => _instance._transformComponentRepresentation;
    
    [SerializeField]
    private GameObject _boxShapeComponentRepresentation = default;
    public static GameObject BoxShapeComponentInScene => _instance._boxShapeComponentRepresentation;

    [SerializeField]
    private GameObject _sphereShapeComponentRepresentation = default;
    public static GameObject SphereShapeComponentInScene => _instance._sphereShapeComponentRepresentation;
    
    [SerializeField]
    private GameObject _planeShapeComponentRepresentation = default;
    public static GameObject PlaneShapeComponentInScene => _instance._planeShapeComponentRepresentation;
    
    [SerializeField]
    private GameObject _cylinderShapeComponentRepresentation = default;
    public static GameObject CylinderShapeComponentInScene => _instance._cylinderShapeComponentRepresentation;
    
    [SerializeField]
    private GameObject _coneShapeComponentRepresentation = default;
    public static GameObject ConeShapeComponentInScene => _instance._coneShapeComponentRepresentation;
    
    [SerializeField]
    private GameObject _gltfShapeComponentRepresentation = default;
    public static GameObject GltfShapeComponentInScene => _instance._gltfShapeComponentRepresentation;

    [Header("UI Representations")]
    [SerializeField]
    private GameObject _transformComponentUI = default;
    public static GameObject TransformComponentUI => _instance._transformComponentUI;
    
    [SerializeField]
    private GameObject _boxShapeComponentUI = default;
    public static GameObject BoxShapeComponentUI => _instance._boxShapeComponentUI;

    [SerializeField]
    private GameObject _sphereShapeComponentUI = default;
    public static GameObject SphereShapeComponentUI => _instance._sphereShapeComponentUI;
    
    [SerializeField]
    private GameObject _planeShapeComponentUI = default;
    public static GameObject PlaneShapeComponentUI => _instance._planeShapeComponentUI;
    
    [SerializeField]
    private GameObject _cylinderShapeComponentUI = default;
    public static GameObject CylinderShapeComponentUI => _instance._cylinderShapeComponentUI;
    
    [SerializeField]
    private GameObject _coneShapeComponentUi = default;
    public static GameObject ConeShapeComponentUI => _instance._coneShapeComponentUi;
    
    [SerializeField]
    private GameObject _gltfShapeComponentUI = default;
    public static GameObject GltfShapeComponentUI => _instance._gltfShapeComponentUI;
    
    public static Type TransformComponentType = typeof(TransformComponent);
    public static Type BoxShapeComponentType = typeof(BoxShapeComponent);
    public static Type SphereShapeComponentType = typeof(SphereShapeComponent);
    public static Type PlaneShapeComponentType = typeof(PlaneShapeComponent);
    public static Type CylinderShapeComponentType = typeof(CylinderShapeComponent);
    public static Type ConeShapeComponentType = typeof(ConeShapeComponent);
    public static Type GltfShapeComponentType = typeof(GLTFShapeComponent);

    public static Dictionary<string, Type> AllComponentTypes = new Dictionary<string, Type>()
    {
        {"Transform", TransformComponentType},
        {"Box Shape", BoxShapeComponentType},
        {"Sphere Shape", SphereShapeComponentType},
        {"Plane Shape", PlaneShapeComponentType},
        {"Cylinder Shape", CylinderShapeComponentType},
        {"Cone Shape", ConeShapeComponentType},
        {"GLTF Shape", GltfShapeComponentType}
    };
    

    //public static Type GetComponentByName(string name)
    //{
    //    return name switch
    //    {
    //        "transform" => TransformComponentType,
    //        "boxShape" => BoxShapeComponentType,
    //        "sphereShape" => SphereShapeComponentType,
    //        "GLTFShape" => GltfShapeComponentType,
    //        _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Component name not found")
    //    };
    //}


    private static ComponentRepresentationList _instance;
    // Start is called before the first frame update
    //void Start()
    //{
    //    _instance = this;
    //}

    public void SetupStatics()
    {
        Debug.Log("init component representation");
        _instance = this; 
    }
}
