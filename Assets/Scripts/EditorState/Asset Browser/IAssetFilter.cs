using Assets.Scripts.EditorState;
using System.Collections.Generic;

/// <summary>
/// This interfaces job is to modify the asset metadata dictionary that gets displayed in the
/// asset manager window.
/// </summary>
public interface IAssetFilter
{
    public enum Slot
    {
        NonExclusive,
        Type,
        Sorting,
    }
    public Slot FilterSlot { get; }
    public void Apply(List<AssetMetadata> metadata);
    public string GetName();
}
