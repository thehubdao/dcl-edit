using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        [SerializeField]
        private EditorStates _editorStates;

        [SerializeField]
        private CameraSystem _cameraSystem;

        [SerializeField]
        private LoadFromVersion1System _loadFromVersion1System;

        [SerializeField]
        private SetupSceneEventListenersSystem _setupSceneEventListenersSystem;

        void Awake()
        {
            EditorStates.Instance = _editorStates;

            _cameraSystem.CameraStartup();

            _loadFromVersion1System.Load();

            _setupSceneEventListenersSystem.SetupSceneEventListeners();
        }   
    }
}
