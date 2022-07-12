using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using UnityEngine;

public class SetupSceneEventListenersSystem : MonoBehaviour
{
    public static void SetupSceneEventListeners()
    {
        var scene = EditorStates.CurrentSceneState.CurrentScene;

        if (scene == null)
            return;

        var sceneListenersToSetup = EditorStates.CurrentUnityState.SceneVisuals.GetComponentsInChildren<ISetupSceneEventListeners>();
        
        foreach (var listenerToSetup in sceneListenersToSetup)
        {
            listenerToSetup.SetupSceneEventListeners();
        }
    }
}
