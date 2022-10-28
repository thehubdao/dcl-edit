using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System.Collections.Generic;
using Zenject;

namespace Assets.Scripts.System
{
    public class AssetBrowserSystem
    {
        public List<IAssetFilter> Filters => _assetBrowserState.filters;

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

        public IEnumerable<AssetMetadata> GetFilteredMetadata()
        {
            var ids = _assetManagerSystem.GetAllAssetIds();
            var allAssets = new List<AssetMetadata>();
            foreach (var id in ids)
            {
                allAssets.Add(_assetManagerSystem.GetMetadataById(id));
            }

            foreach (var f in _assetBrowserState.filters)
            {
                f.Apply(allAssets);
            }

            return allAssets;
        }

        public void AddFilter(IAssetFilter newFilter)
        {
            foreach (var f in _assetBrowserState.filters)
            {
                if (f.FilterSlot == newFilter.FilterSlot) return;
            }

            _assetBrowserState.filters.Add(newFilter);
            _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
        }

        public void RemoveFilter(IAssetFilter filter) => _assetBrowserState.filters.Remove(filter);
    }
}
