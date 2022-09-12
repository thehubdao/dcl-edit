using System.IO;
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

            // Load scene
            var v2Path = EditorStates.CurrentPathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            var scene = Directory.Exists(v2Path) ?
                SceneLoadSaveSystem.Load(v2Path) :
                LoadFromVersion1System.Load();

            SetupSceneSystem.SetupScene(scene);
        }

        void Start()
        {
            WorkspaceSaveSystem.Load(_editorStates.UnityState.dynamicPanelsCanvas);
        }
    }
}