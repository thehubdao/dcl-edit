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
        private SetupSceneEventListenersSystem _setupSceneEventListenersSystem;

        void Awake()
        {
            EditorStates.Instance = _editorStates;

            var scene = LoadFromVersion1System.Load();

            SetupSceneSystem.SetupScene(scene);
        }

        void Start()
        {
            WorkspaceSaveSystem.Load(_editorStates.UnityState.dynamicPanelsCanvas);
        }
    }
}