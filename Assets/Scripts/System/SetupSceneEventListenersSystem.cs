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
    private SceneFile _sceneFile;

    [Inject]
    private void Construct(UnityState unityState, SceneFile sceneFile)
    {
        _unityState = unityState;
        _sceneFile = sceneFile;
    }

    public void SetupSceneEventListeners()
    {
        var scene = _sceneFile.CurrentScene;

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
