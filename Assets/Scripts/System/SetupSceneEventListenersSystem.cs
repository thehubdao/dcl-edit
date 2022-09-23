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
    private SceneState _sceneState;

    [Inject]
    private void Construct(UnityState unityState, SceneState sceneState)
    {
        _unityState = unityState;
        _sceneState = sceneState;
    }

    public void SetupSceneEventListeners()
    {
        var scene = _sceneState.CurrentScene;

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
