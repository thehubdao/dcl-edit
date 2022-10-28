using Assets.Scripts.EditorState;
using System.Collections.Generic;

public class AssetNameSortingFilter : IAssetFilter
{
    public IAssetFilter.Slot FilterSlot => IAssetFilter.Slot.Sorting;
    private bool sortAscending;

    public AssetNameSortingFilter(bool sortAscending = true)
    {
        this.sortAscending = sortAscending;
    }

    public void Apply(List<AssetMetadata> metadata)
    {
        int CompareAscending(AssetMetadata a, AssetMetadata b) => a.assetDisplayName.CompareTo(b.assetDisplayName);
        int CompareDescending(AssetMetadata a, AssetMetadata b) => a.assetDisplayName.CompareTo(b.assetDisplayName);

        if (sortAscending)
        {
            metadata.Sort(CompareAscending);
        }
        else
        {
            metadata.Sort(CompareDescending);
        }
    }

    public string GetName() => sortAscending ? "Name: A-Z" : "Name: Z-A";
}
