using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;
using static AssetThumbnailGeneratorState;

public class AssetThumbnailGeneratorSystem
{
    // Dependencies
    AssetManagerSystem assetManagerSystem;
    AssetThumbnailGeneratorState state;
    EditorEvents editorEvents;

    [Inject]
    private void Construct(AssetThumbnailGeneratorState assetThumbnailGeneratorState, AssetManagerSystem assetManagerSystem, EditorEvents editorEvents)
    {
        state = assetThumbnailGeneratorState;
        this.assetManagerSystem = assetManagerSystem;
        this.editorEvents = editorEvents;

        this.editorEvents.onAssetDataUpdatedEvent += OnAssetDataUpdatedCallback;
        this.editorEvents.onAssetDataUpdatedEvent += OnSceneDataUpdatedCallback;
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

    void OnSceneDataUpdatedCallback(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            foreach (var queuedAsset in state.waitingForAssetData)
            {
                var assetData = assetManagerSystem.GetDataById(queuedAsset.id);
                if (assetData is not SceneAssetData sceneAssetData) continue;
                if (sceneAssetData.assetList.All(i => i.Key != id)) continue;
                    
                state.waitingForAssetData.Remove(queuedAsset);
                        
                state.queuedAssets.Enqueue(queuedAsset);
                        
                editorEvents.InvokeStartThumbnailGeneration();
            }
        }
    }
}