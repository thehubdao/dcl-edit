using Assets.Scripts.EditorState;

/// <summary>
/// This interfaces job is to filter the asset metadata that gets displayed in the asset browser.
/// </summary>
public interface IAssetFilter
{
    public AssetHierarchyItem Apply(AssetHierarchyItem metadata);
    public string GetName();
    public bool Equals(IAssetFilter other);
}
