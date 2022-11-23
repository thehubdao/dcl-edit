using Assets.Scripts.EditorState;
using System.Collections.Generic;
using System.Linq;

public class AssetTypeFilter : IAssetFilter
{
    public AssetMetadata.AssetType type;

    public AssetTypeFilter(AssetMetadata.AssetType type)
    {
        this.type = type;
    }

    public List<AssetMetadata> Apply(List<AssetMetadata> metadata)
    {
        var filteredData = new List<AssetMetadata>();
        metadata.ForEach(item => filteredData.Add(item));
        return metadata.Where(metadata => metadata.assetType == type).ToList();
    }

    public string GetName() => type.ToString();
    public bool Equals(IAssetFilter other)
    {
        if (other is AssetTypeFilter otherTypeFilter)
        {
            if (otherTypeFilter.type == type)
            {
                return true;
            }
        }
        return false;
    }
}
