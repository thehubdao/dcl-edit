using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Assets.Scripts.System
{
    public class AssetBrowserSystem
    {
        public List<IAssetFilter> Filters => _assetBrowserState.filters;
        public IAssetSorting Sorting => _assetBrowserState.sorting;

        // Dependencies
        private AssetManagerSystem _assetManagerSystem;
        private AssetBrowserState _assetBrowserState;
        private EditorEvents _editorEvents;

        [Inject]
        public void Construct(AssetManagerSystem assetManagerSystem, AssetBrowserState assetBrowserState, EditorEvents editorEvents)
        {
            _assetManagerSystem = assetManagerSystem;
            _assetBrowserState = assetBrowserState;
            _editorEvents = editorEvents;
        }

        public List<AssetMetadata> GetFilteredMetadata()
        {
            var ids = _assetManagerSystem.GetAllAssetIds();
            var allAssets = new List<AssetMetadata>();
            foreach (var id in ids)
            {
                allAssets.Add(_assetManagerSystem.GetMetadataById(id));
            }

            if (Filters.Count > 0)
            {
                var filteredAssets = new List<AssetMetadata>();
                foreach (var f in _assetBrowserState.filters)
                {
                    var filtered = f.Apply(allAssets);
                    filteredAssets = filteredAssets.Union(filtered).ToList();
                }
                allAssets = filteredAssets;
            }

            allAssets = Sorting?.Apply(allAssets);

            return allAssets;
        }

        public void AddFilter(IAssetFilter newFilter)
        {
            foreach (var f in _assetBrowserState.filters)
            {
                if (f.Equals(newFilter)) return;
            }

            _assetBrowserState.filters.Add(newFilter);
            _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
        }

        public void RemoveFilter(IAssetFilter filter) => _assetBrowserState.filters.Remove(filter);

        public void ChangeSorting(IAssetSorting newSorting)
        {
            if (newSorting != null)
            {
                _assetBrowserState.sorting = newSorting;
                _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
        }
    }
}
