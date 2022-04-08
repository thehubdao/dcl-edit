using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        [SerializeField]
        EditorStates _editorStates;

        [SerializeField]
        CameraSystem _cameraSystem;

        void Awake()
        {
            EditorStates.Instance = _editorStates;

            _cameraSystem.CameraStartup();
        }
    }
}
