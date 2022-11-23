using Assets.Scripts.EditorState;
using System.Collections.Generic;

/// <summary>
/// This interfaces job is to sort the asset metadata that gets displayed in the asset browser.
/// </summary>
public interface IAssetSorting
{
    public bool SortAscending { get; set; }
    public List<AssetMetadata> Apply(List<AssetMetadata> metadata);
    public string GetName();
}
