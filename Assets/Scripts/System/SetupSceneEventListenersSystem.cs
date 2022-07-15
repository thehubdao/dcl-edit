using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine;

public class SetupSceneEventListenersSystem : MonoBehaviour
{
    public static void SetupSceneEventListeners()
    {
        var scene = EditorStates.CurrentSceneState.CurrentScene;

        if (scene == null)
            return;

        var sceneListenersToSetup = 
            EditorStates.CurrentUnityState.SceneVisuals.GetComponentsInChildren<ISetupSceneEventListeners>()
                .Concat(EditorStates.CurrentUnityState.Ui.GetComponentsInChildren<ISetupSceneEventListeners>());

        foreach (var listenerToSetup in sceneListenersToSetup)
        {
            listenerToSetup.SetupSceneEventListeners();
        }
    }
}
