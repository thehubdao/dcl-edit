using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRepresentationList : MonoBehaviour
{

    [Header("Scene Representations")]
    [SerializeField]
    private GameObject _transformComponentInScene;
    public static GameObject TransformComponentInScene => _instance._transformComponentInScene;
    
    [SerializeField]
    private GameObject _boxShapeComponentInScene;
    public static GameObject BoxShapeComponentInScene => _instance._boxShapeComponentInScene;

    [SerializeField]
    private GameObject _sphereShapeComponentInScene;
    public static GameObject SphereShapeComponentInScene => _instance._sphereShapeComponentInScene;
    
    [SerializeField]
    private GameObject _gltfShapeComponentInScene;
    public static GameObject GltfShapeComponentInScene => _instance._gltfShapeComponentInScene;

    [Header("UI Representations")]
    [SerializeField]
    private GameObject _transformComponentUI;
    public static GameObject TransformComponentUI => _instance._transformComponentUI;
    
    [SerializeField]
    private GameObject _boxShapeComponentUI;
    public static GameObject BoxShapeComponentUI => _instance._boxShapeComponentUI;

    [SerializeField]
    private GameObject _sphereShapeComponentUI;
    public static GameObject SphereShapeComponentUI => _instance._sphereShapeComponentUI;
    
    [SerializeField]
    private GameObject _gltfShapeComponentUI;
    public static GameObject GltfShapeComponentUI => _instance._gltfShapeComponentUI;
    
    public static Type TransformComponentComponent = typeof(TransformComponent);
    public static Type BoxShapeComponentComponent = typeof(BoxShapeComponent);
    public static Type SphereShapeComponentComponent = typeof(SphereShapeComponent);
    public static Type GltfShapeComponentComponent = typeof(GLTFShapeComponent);

    public static Dictionary<string, Type> AllComponentComponents = new Dictionary<string, Type>()
    {
        {"Transform", TransformComponentComponent},
        {"Box Shape", BoxShapeComponentComponent},
        {"Sphere Shape", SphereShapeComponentComponent},
        {"GLTF Shape", GltfShapeComponentComponent}
    };
    

    public static Type GetComponentByName(string name)
    {
        return name switch
        {
            "transform" => TransformComponentComponent,
            "boxShape" => BoxShapeComponentComponent,
            "sphereShape" => SphereShapeComponentComponent,
            "GLTFShape" => GltfShapeComponentComponent,
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
