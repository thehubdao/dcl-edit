using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Zenject;

public class DialogSystem
{
    // Dependencies
    private DialogState dialogState;
    private EditorEvents editorEvents;
    private AssetBrowserState assetBrowserState;

    [Inject]
    void Construct(DialogState dialogState, EditorEvents editorEvents, AssetBrowserState assetBrowserState)
    {
        this.dialogState = dialogState;
        this.editorEvents = editorEvents;
        this.assetBrowserState = assetBrowserState;
    }

    /// <summary>
    /// Lets the user select an asset which will then be added to a component on the entity with the targetEntityId.
    /// </summary>
    /// <param name="targetEntityId"></param>
    /// <param name="component">The component which this asset will be added to.</param>
    public void OpenAssetDialog(DclComponent component)
    {
        dialogState.currentDialog.Value = DialogState.DialogType.Asset;
        dialogState.targetComponent = component;

        assetBrowserState.StoreShownTypesTemp();
        switch (component.NameInCode)
        {
            case "GLTFShape":
                assetBrowserState.AddShownType(AssetMetadata.AssetType.Model);
                break;
            case "Scene":
                assetBrowserState.AddShownType(AssetMetadata.AssetType.Scene);
                break;
            default:
                break;
        }
    }

    public void CloseCurrentDialog()
    {
        dialogState.currentDialog.Value = DialogState.DialogType.None;

        assetBrowserState.RestoreShownTypes();
    }

    public bool IsMouseOverDialog() => dialogState.mouseOverDialogWindow;
}
