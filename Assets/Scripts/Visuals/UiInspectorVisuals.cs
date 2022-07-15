using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Visuals;
using UnityEngine;

public class UiInspectorVisuals : MonoBehaviour, ISetupSceneEventListeners
{
    [SerializeField]
    private GameObject _content;

    public void SetupSceneEventListeners()
    {
        EditorStates.CurrentSceneState.CurrentScene?.SelectionState.SelectionChangedEvent.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Debug.Log("Update ui vis");

        new UiBuilder()
            .Title("Inspect me,")
            .Title("Daddy")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .Title("Test")
            .ClearAndMake(_content);
    }
}
