using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRepresentationList : MonoBehaviour
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
    private GameObject _gltfShapeComponentUI = default;
    public static GameObject GltfShapeComponentUI => _instance._gltfShapeComponentUI;
    
    public static Type TransformComponentType = typeof(TransformComponent);
    public static Type BoxShapeComponentType = typeof(BoxShapeComponent);
    public static Type SphereShapeComponentType = typeof(SphereShapeComponent);
    public static Type GltfShapeComponentType = typeof(GLTFShapeComponent);

    public static Dictionary<string, Type> AllComponentTypes = new Dictionary<string, Type>()
    {
        {"Transform", TransformComponentType},
        {"Box Shape", BoxShapeComponentType},
        {"Sphere Shape", SphereShapeComponentType},
        {"GLTF Shape", GltfShapeComponentType}
    };
    

    public static Type GetComponentByName(string name)
    {
        return name switch
        {
            "transform" => TransformComponentType,
            "boxShape" => BoxShapeComponentType,
            "sphereShape" => SphereShapeComponentType,
            "GLTFShape" => GltfShapeComponentType,
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Component name not found")
        };
    }


    private static ComponentRepresentationList _instance;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }
}
