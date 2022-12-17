using System.Security.Policy;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class SetupSceneSystem
    {
        // Dependencies
        //private CameraSystem _cameraSystem;
        //private SetupSceneEventListenersSystem _setupSceneEventListenersSystem;
        //private EditorState.SceneDirectoryState _sceneDirectoryState;
        //private ISceneLoadSystem _sceneLoadSystem;
        //
        //[Inject]
        //private void Construct(
        //    CameraSystem cameraSystem,
        //    SetupSceneEventListenersSystem setupSceneEventListenersSystem,
        //    EditorState.SceneDirectoryState sceneDirectoryState,
        //    ISceneLoadSystem sceneLoadSystem)
        //{
        //    _cameraSystem = cameraSystem;
        //    _setupSceneEventListenersSystem = setupSceneEventListenersSystem;
        //    _sceneDirectoryState = sceneDirectoryState;
        //    _sceneLoadSystem = sceneLoadSystem;
        //}
        //
        //public void SetupScene(string path)
        //{
        //    _sceneDirectoryState.directoryPath = path;
        //
        //    _sceneLoadSystem.Load(_sceneDirectoryState);
        //
        //    _cameraSystem.CameraStartup();
        //
        //    _setupSceneEventListenersSystem.SetupSceneEventListeners();
        //}
    }
}
