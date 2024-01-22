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

        public bool isExpanded
        {
            get => isExpandedInternal;
            set
            {
                isExpandedInternal = value;
                InvokeChange();
            }
        }

        private readonly List<AbStructItem> items = new();

        private bool isExpandedInternal = false;

        public void Clear()
        {
            items.Clear();
        }

        [CanBeNull]
        public AbStructFolder GetChildFolder(string folderName)
        {
            return (AbStructFolder) items.Find(si => si.name == folderName && si.GetType() == typeof(AbStructFolder));
        }

        public AbStructFolder CreateFolderNoChangeInvoke(string folderName)
        {
            var abStructItem = new AbStructFolder(folderName);
            items.Add(abStructItem);
            return abStructItem;
        }

        public AbStructFolder GetOrCreateFolderNoChangeInvoke(string folderName)
        {
            return GetChildFolder(folderName) ?? CreateFolderNoChangeInvoke(folderName);
        }

        public List<AbStructItem> GetItems()
        {
            return items;
        }

        public void AddAssetNoChangeInvoke(CommonAssetTypes.AssetInfo assetInfo)
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

        foreach (var assetInfo in discoveredAssets.discoveredAssets.Values)
        {
            if (!assetInfo.visible)
                continue;

            var path = new List<string> {AssetSourceName(assetInfo.assetSource)};
            path.AddRange(assetInfo.displayPath.Split("/"));

            var folder = path.Aggregate(rootItem, (current, pathPart) => current.GetOrCreateFolderNoChangeInvoke(pathPart));

            folder.AddAssetNoChangeInvoke(assetInfo);
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
