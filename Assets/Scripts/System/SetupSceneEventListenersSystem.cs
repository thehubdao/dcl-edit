using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

public class SetupSceneEventListenersSystem
{
    // Dependencies
    private UnityState _unityState;
    private SceneDirectoryState _sceneDirectoryState;

    [Inject]
    private void Construct(UnityState unityState, SceneDirectoryState sceneDirectoryState)
    {
        _unityState = unityState;
        _sceneDirectoryState = sceneDirectoryState;
    }

    public void SetupSceneEventListeners()
    {
        var scene = _sceneDirectoryState.CurrentScene;

        if (scene == null)
            return;

        var sceneListenersToSetup = 
            _unityState.SceneVisuals.GetComponentsInChildren<ISetupSceneEventListeners>()
                .Concat(_unityState.Ui.GetComponentsInChildren<ISetupSceneEventListeners>());

        foreach (var listenerToSetup in sceneListenersToSetup)
        {
            listenerToSetup.SetupSceneEventListeners();
        }
    }
}
