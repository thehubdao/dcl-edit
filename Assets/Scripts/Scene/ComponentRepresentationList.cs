using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRepresentationList : MonoBehaviour
{
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


    private static ComponentRepresentationList _instance;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }
}
