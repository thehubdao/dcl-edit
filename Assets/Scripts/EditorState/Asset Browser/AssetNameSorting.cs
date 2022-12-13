using Assets.Scripts.EditorState;
using System.Collections.Generic;
using System.Linq;

public class AssetNameSorting : IAssetSorting
{
    public bool SortAscending { get; set; }

    public AssetNameSorting(bool sortAscending = true)
    {
        SortAscending = sortAscending;
    }

    public List<AssetMetadata> Apply(List<AssetMetadata> metadata)
    {
        if (SortAscending)
        {
            return metadata.OrderBy(item => item.assetDisplayName).ToList();
        }
        return metadata.OrderByDescending(item => item.assetDisplayName).ToList();
    }

    public string GetName() => SortAscending ? "Name:\nA-Z" : "Name:\nZ-A";
}
