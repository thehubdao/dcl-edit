using Assets.Scripts.Events;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AssetCacheSystem
{
    Dictionary<Guid, AssetCacheEntry> entries = new Dictionary<Guid, AssetCacheEntry>();
    // Used by the editor window
    public Dictionary<Guid, AssetCacheEntry> Entries => entries;
    public static string modelCachePath => Path.Combine(Application.persistentDataPath, "dcl-edit/builder_assets/");

    // Dependencies
    private LoadGltfFromFileSystem gltfLoader;
    private EditorEvents editorEvents;
    private BuilderAssetDownLoader assetDownloader;
    private LoadedModelFormat.Factory loadedModelFormatFactory;


    [Inject]
    public void Construct(LoadGltfFromFileSystem gltfLoader, EditorEvents editorEvents, IWebRequestSystem webRequestSystem, LoadedModelFormat.Factory loadedModelFormatFactory)
    {
        this.gltfLoader = gltfLoader;
        this.editorEvents = editorEvents;
        this.assetDownloader = new BuilderAssetDownLoader(modelCachePath, webRequestSystem);
        this.loadedModelFormatFactory = loadedModelFormatFactory;
    }

    public void Add(Guid id, AssetFormat format)
    {
        if (entries.TryGetValue(id, out var entry))
        {
            // Replace format, if already exists (e.g. never have multiple MetadataFileFormats)
            for (int i = entry.formats.Count - 1; i >= 0; i--)
            {
                if (entry.formats[i].GetType() == format.GetType())
                {
                    entry.formats.RemoveAt(i);
                }
            }

            // Add format to existing entry
            entry.formats.Add(format);
        }
        else
        {
            // Create new entry
            var newEntry = new AssetCacheEntry();
            newEntry.formats.Add(format);
            this.entries.Add(id, newEntry);
        }

        editorEvents.InvokeAssetDataUpdatedEvent(id);
    }

    public void Remove(Guid id)
    {
        if (entries.ContainsKey(id))
        {
            entries.Remove(id);
        }
    }

}
