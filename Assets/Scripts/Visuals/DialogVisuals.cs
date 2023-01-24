using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using Assets.Scripts.Visuals.UiHandler;
using System;
using UnityEngine;
using Zenject;

public class DialogVisuals : MonoBehaviour
{
    // Dependencies
    DialogSystem dialogSystem;
    DialogState dialogState;
    UnityState unityState;
    EditorEvents editorEvents;
    SceneManagerSystem sceneManagerSystem;

    [Inject]
    void Construct(
        DialogSystem dialogSystem,
        DialogState dialogState,
        UnityState unityState,
        EditorEvents editorEvents,
        SceneManagerSystem sceneManagerSystem)
    {
        this.dialogSystem = dialogSystem;
        this.dialogState = dialogState;
        this.unityState = unityState;
        this.editorEvents = editorEvents;
        this.sceneManagerSystem = sceneManagerSystem;

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
            DclEntity entity = scene.GetEntityById(dialogState.targetEntityId);
            var shapeComponent = entity.GetComponentBySlot("Shape");
            if (shapeComponent != null) entity.RemoveComponent(shapeComponent);
            entity.AddComponent(new DclGltfShapeComponent(assetId));
            editorEvents.InvokeSelectionChangedEvent();

            dialogSystem.CloseCurrentDialog();
        };
    }
}
