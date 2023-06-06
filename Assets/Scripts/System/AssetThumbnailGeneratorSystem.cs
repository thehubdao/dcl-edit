using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static AssetThumbnailGeneratorState;

public class AssetThumbnailGeneratorSystem
{
    // Dependencies
    AssetThumbnailGeneratorState state;
    EditorEvents editorEvents;

    [Inject]
    private void Construct(AssetThumbnailGeneratorState assetThumbnailGeneratorState, EditorEvents editorEvents)
    {
        state = assetThumbnailGeneratorState;
        this.editorEvents = editorEvents;

        this.editorEvents.onAssetDataUpdatedEvent += OnAssetDataUpdatedCallback;
    }

    public void Generate(Guid id, Action<Texture2D> then)
    {
        if (state.queuedAssets.Any(qa => qa.id == id) ||
            state.waitingForAssetData.Any(qa => qa.id == id))
        {
            return;
        }

        state.queuedAssets.Enqueue(new QueuedAsset(id, then));
        
        editorEvents.InvokeStartThumbnailGeneration();
    }

    void OnAssetDataUpdatedCallback(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            if (state.waitingForAssetData.Any(qa => qa.id == id))
            {
                var qa = state.waitingForAssetData.Find(qa => qa.id == id);
                state.waitingForAssetData.Remove(qa);

                state.queuedAssets.Enqueue(qa);

                editorEvents.InvokeStartThumbnailGeneration();
            }
        }
    }
}