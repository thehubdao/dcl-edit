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
            .Text("This is some text")
            .Text("Some other text")
            .Spacer(30)
            .Text("More text but after a space")
            .StringPropertyInput("Some input", "Some input", "This is the default stuff")
            .StringPropertyInput("Some other input", "Some input", "This is the default")
            .StringPropertyInput("This is also some input", "Some input", "This is the")
            .StringPropertyInput("And this is empty", "Some input", "")
            .NumberPropertyInput("Here you can type in numbers", "number", 10)
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .Text("More text but after a space")
            .ClearAndMake(_content);
    }
}
