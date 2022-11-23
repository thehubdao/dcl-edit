using System.Collections.Generic;

namespace Assets.Scripts.EditorState
{
    public class AssetBrowserState
    {
        public List<IAssetFilter> filters = new List<IAssetFilter>();
        public IAssetSorting sorting;
    }
}
