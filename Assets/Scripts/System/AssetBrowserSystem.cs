using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Assets;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class AssetBrowserSystem
{
    public abstract class AbStructItem
    {
        public abstract string name { get; }
        public event Action Change;

        public void InvokeChange()
        {
            Change?.Invoke();
        }
    }

    public class AbStructAsset : AbStructItem
    {
        public override string name => assetInfo.assetName;

        public CommonAssetTypes.AssetInfo assetInfo;

        public AbStructAsset(CommonAssetTypes.AssetInfo assetInfo)
        {
            this.assetInfo = assetInfo;
        }
    }

    public class AbStructFolder : AbStructItem
    {
        public AbStructFolder(string name)
        {
            this.name = name;
        }

        public override string name { get; }

        private readonly List<AbStructItem> items = new();

        public void Clear()
        {
            items.Clear();
        }

        [CanBeNull]
        public AbStructFolder GetChildFolder(string folderName)
        {
            return (AbStructFolder) items.Find(si => si.name == folderName && si.GetType() == typeof(AbStructFolder));
        }

        public AbStructFolder CreateFolder(string folderName)
        {
            var abStructItem = new AbStructFolder(folderName);
            items.Add(abStructItem);
            return abStructItem;
        }

        public AbStructFolder GetOrCreateFolder(string folderName)
        {
            return GetChildFolder(folderName) ?? CreateFolder(folderName);
        }

        public List<AbStructItem> GetItems()
        {
            return items;
        }

        public void AddAsset(CommonAssetTypes.AssetInfo assetInfo)
        {
            items.Add(new AbStructAsset(assetInfo));
        }
    }

    public AbStructFolder rootItem { get; } = new("");

    // Dependencies
    private DiscoveredAssets discoveredAssets;

    [Inject]
    private void Construct(DiscoveredAssets discoveredAssets)
    {
        this.discoveredAssets = discoveredAssets;

        this.discoveredAssets.discoveredAssets.OnDictionaryChanged += UpdateEntireStruct;

        UpdateEntireStruct();
    }


    private void UpdateEntireStruct()
    {
        rootItem.Clear();

        var tmpCount = 0;

        foreach (var assetInfo in discoveredAssets.discoveredAssets.Values)
        {
            var path = new List<string> {AssetSourceName(assetInfo.assetSource)};
            path.AddRange(assetInfo.displayPath.Split("/"));

            var folder = path.Aggregate(rootItem, (current, pathPart) => current.GetOrCreateFolder(pathPart));

            folder.AddAsset(assetInfo);

            if (tmpCount++ > 1000)
                break;
        }

        rootItem.InvokeChange();
    }

    private string AssetSourceName(CommonAssetTypes.AssetSource assetSource)
    {
        return assetSource switch
        {
            CommonAssetTypes.AssetSource.Unknown => "unknown",
            CommonAssetTypes.AssetSource.DecentralandBuilder => "Decentraland Builder",
            CommonAssetTypes.AssetSource.Local => "Local Asset Folder",
            _ => throw new ArgumentOutOfRangeException(nameof(assetSource), assetSource, null)
        };
    }
}