using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiHandler;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DialogVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Dependencies
    DialogSystem dialogSystem;
    DialogState dialogState;
    UnityState unityState;
    EditorEvents editorEvents;
    SceneManagerSystem sceneManagerSystem;
    CommandSystem commandSystem;

    [Inject]
    void Construct(
        DialogSystem dialogSystem,
        DialogState dialogState,
        UnityState unityState,
        EditorEvents editorEvents,
        SceneManagerSystem sceneManagerSystem,
        CommandSystem commandSystem)
    {
        this.dialogSystem = dialogSystem;
        this.dialogState = dialogState;
        this.unityState = unityState;
        this.editorEvents = editorEvents;
        this.sceneManagerSystem = sceneManagerSystem;
        this.commandSystem = commandSystem;

        editorEvents.onDialogChangedEvent += UpdateDialog;
    }

    private void UpdateDialog()
    {
        switch (dialogState.currentDialog)
        {
            case DialogState.DialogType.None:
                RemoveChildObjects();
                break;
            case DialogState.DialogType.Asset:
                ShowAssetDialog();
                break;
            default:
                break;
        }
    }

    void RemoveChildObjects()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void ShowAssetDialog()
    {
        GameObject window = Instantiate(unityState.assetDialog, transform);
        AssetDialogHandler handler = window.GetComponent<AssetDialogHandler>();
        handler.assetBrowserVisuals.assetButtonOnClickOverride = (Guid assetId) =>
        {
            DclScene scene = sceneManagerSystem.GetCurrentScene();

            // Update the target component with the new asset
            var sceneProperty = dialogState.targetComponent.GetPropertyByName("scene");
            var assetProperty = dialogState.targetComponent.GetPropertyByName("asset");
            if (sceneProperty != null)
            {
                var oldValue = sceneProperty.GetConcrete<Guid>().FixedValue;
                var identifier = new DclPropertyIdentifier(dialogState.targetComponent.Entity.Id, dialogState.targetComponent.NameInCode, "scene");
                commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangePropertyCommand(identifier, oldValue, assetId));
            }
            if (assetProperty != null)
            {
                var oldValue = assetProperty.GetConcrete<Guid>().FixedValue;
                var identifier = new DclPropertyIdentifier(dialogState.targetComponent.Entity.Id, dialogState.targetComponent.NameInCode, "asset");
                commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangePropertyCommand(identifier, oldValue, assetId));
            }

            dialogSystem.CloseCurrentDialog();
            editorEvents.InvokeSelectionChangedEvent();
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dialogState.currentDialog != DialogState.DialogType.None) dialogState.mouseOverDialogWindow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dialogState.mouseOverDialogWindow = false;
    }
}
