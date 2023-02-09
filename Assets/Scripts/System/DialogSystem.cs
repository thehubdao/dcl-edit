using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using Zenject;

public class DialogSystem
{
    // Dependencies
    private DialogState dialogState;
    private EditorEvents editorEvents;

    [Inject]
    void Construct(DialogState dialogState, EditorEvents editorEvents)
    {
        this.dialogState = dialogState;
        this.editorEvents = editorEvents;
    }

    /// <summary>
    /// Lets the user select an asset which will then be added to a component on the entity with the targetEntityId.
    /// </summary>
    /// <param name="targetEntityId"></param>
    /// <param name="component">The component which this asset will be added to.</param>
    public void OpenAssetDialog(DclComponent component)
    {
        dialogState.currentDialog = DialogState.DialogType.Asset;
        dialogState.targetComponent = component;
        editorEvents.InvokeDialogChangedEvent();
    }

    public void CloseCurrentDialog()
    {
        dialogState.currentDialog = DialogState.DialogType.None;
        editorEvents.InvokeDialogChangedEvent();
    }

    public bool IsMouseOverDialog() => dialogState.mouseOverDialogWindow;
}
