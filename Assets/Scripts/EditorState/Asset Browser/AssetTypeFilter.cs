using Assets.Scripts.EditorState;

public class AssetTypeFilter : IAssetFilter
{
    public AssetMetadata.AssetType type;

    public AssetTypeFilter(AssetMetadata.AssetType type)
    {
        this.type = type;
    }

    public AssetHierarchyItem Apply(AssetHierarchyItem item)
    {
        AssetHierarchyItem filteredItem = new AssetHierarchyItem(item.name);

        foreach (AssetHierarchyItem subdir in item.childDirectories)
        {
            filteredItem.childDirectories.Add(Apply(subdir));
        }

        for (int i = item.assets.Count - 1; i > 0; i--)
        {
            AssetMetadata asset = item.assets[i];
            if (asset.assetType == type)
            {
                filteredItem.assets.Add(asset);
            }
        }

        return filteredItem;
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
