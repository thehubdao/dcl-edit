using System.IO;
using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

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

        // dependencies
        private ISceneLoadSystem _sceneLoadSystem;

        [Inject]
        private void Construct(ISceneLoadSystem sceneSave)
        {
            _sceneLoadSystem = sceneSave;
        }

        void Awake()
        {
            EditorStates.Instance = _editorStates;

            // Load scene
            var v2Path = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            var scene = Directory.Exists(v2Path) ?
                _sceneLoadSystem.Load(v2Path) :
                LoadFromVersion1System.Load();

            SetupSceneSystem.SetupScene(scene);
        }

        void Start()
        {
            WorkspaceSaveSystem.Load(_editorStates.UnityState.dynamicPanelsCanvas);
        }
    }
}