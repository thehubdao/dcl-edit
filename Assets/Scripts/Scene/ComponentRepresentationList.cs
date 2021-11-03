using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRepresentationList : MonoBehaviour
{
    [Header("Scene Representations")]
    [SerializeField]
    private GameObject _transformComponent;
    public static GameObject TransformComponent => _instance._transformComponent;
    
    [SerializeField]
    private GameObject _boxShapeComponent;
    public static GameObject BoxShapeComponent => _instance._boxShapeComponent;

    [SerializeField]
    private GameObject _sphereShapeComponent;
    public static GameObject SphereShapeComponent => _instance._sphereShapeComponent;
    
    [SerializeField]
    private GameObject _gltfShapeComponent;
    public static GameObject GltfShapeComponent => _instance._gltfShapeComponent;

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

    private static ComponentRepresentationList _instance;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }
}
