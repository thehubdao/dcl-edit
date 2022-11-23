using Assets.Scripts.EditorState;
using System.Collections.Generic;

/// <summary>
/// This interfaces job is to filter the asset metadata that gets displayed in the asset browser.
/// </summary>
public interface IAssetFilter
{
    public List<AssetMetadata> Apply(List<AssetMetadata> metadata);
    public string GetName();
    public bool Equals(IAssetFilter other);
}
