using System;
using System.Collections.Generic;

namespace Assets.Scripts.EditorState
{
    public class FileAssetLoaderState
    {
        /// <summary>
        /// The IDs of the assets that were found by this FileAssetLoaderSystem
        /// </summary>
        public List<Guid> assetIds = new List<Guid>();

        /// <summary>
        /// The hierarchy in which the assets appear in the asset browser.
        /// </summary>
        public AssetHierarchyItem assetHierarchy = new AssetHierarchyItem();
    }
}
