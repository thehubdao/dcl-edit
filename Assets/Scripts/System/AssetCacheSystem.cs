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

    /// <summary>
    /// Tries to find the loaded model in the cache. If it doesn't exist yet, existing formats will be converted into a loaded model if possible.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<GameObject> GetLoadedModel(Guid id)
    {
        if (entries.TryGetValue(id, out var entry))
        {
            await entry.conversionSemaphore.WaitAsync();

            GameObject model = GetLoadedModel(entry) ?? await ConvertToLoadedModel(id, entry);

            // Allow others to start a conversion
            entry.conversionSemaphore.Release();

            return model;
        }
        return null;
    }
    public static GameObject CreateCopy(GameObject obj)
    {
        if (obj == null) return null;

        var copy = UnityEngine.Object.Instantiate(obj);
        copy.SetActive(true);
        copy.transform.SetParent(null);
        return copy;
    }

    private GameObject GetLoadedModel(AssetCacheEntry entry)
    {
        foreach (var format in entry.formats)
        {
            if (format is LoadedModelFormat lmf)
            {
                return CreateCopy(lmf.model);
            }
        }
        return null;
    }

    private async Task<GameObject> ConvertToLoadedModel(Guid id, AssetCacheEntry entry)
    {
        foreach (var format in entry.formats)
        {
            if (format is ILoadedModelConvertible conv)
            {
                var loadedFormat = await conv.ConvertToLoadedModelFormat(gltfLoader, assetDownloader, loadedModelFormatFactory);
                if (loadedFormat != null)
                {
                    this.Add(id, loadedFormat);
                    return CreateCopy(loadedFormat.model);
                }
            }
        }
        return null;
    }
}