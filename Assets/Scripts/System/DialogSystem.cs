using Assets.Scripts.Events;
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
    /// Lets the user select an asset which will then be added to a GLTFShapeComponent on the entity with the targetEntityId.
    /// </summary>
    /// <param name="targetEntityId"></param>
    public void OpenAssetDialog(Guid targetEntityId)
    {
        dialogState.currentDialog = DialogState.DialogType.Asset;
        dialogState.targetEntityId = targetEntityId;
        editorEvents.InvokeDialogChangedEvent();
    }

    public void CloseCurrentDialog()
    {
        dialogState.currentDialog = DialogState.DialogType.None;
        editorEvents.InvokeDialogChangedEvent();
    }

    public bool IsMouseOverDialog() => dialogState.mouseOverDialogWindow;
}
