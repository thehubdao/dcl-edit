using Assets.Scripts.EditorState;
using System.Collections.Generic;
using System.Linq;

public class AssetTypeFilter : IAssetFilter
{
    public IAssetFilter.Slot FilterSlot => IAssetFilter.Slot.Type;
    private AssetMetadata.AssetType[] assetTypes;


    public AssetTypeFilter(params AssetMetadata.AssetType[] assetTypes)
    {
        this.assetTypes = assetTypes;
    }

    public void Apply(List<AssetMetadata> metadata)
    {
        for (int i = metadata.Count - 1; i >= 0; i--)
        {
            if (!assetTypes.Contains(metadata[i].assetType))
            {
                metadata.RemoveAt(i);
            }
        }
    }

    public string GetName() => "Type";
}
