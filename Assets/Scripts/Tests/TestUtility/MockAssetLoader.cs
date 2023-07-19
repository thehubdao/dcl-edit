using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockAssetLoader : IAssetLoaderSystem
    {
        // Dependencies
        AssetCacheSystem assetCacheSystem;

        private FileAssetLoaderState loaderState = new FileAssetLoaderState();
        public bool allAssetsCached = false;
        TestData[] testData;

        public struct TestData
        {
            public Guid id;
            public string filename;
            public AssetMetadata.AssetType type;
        }

        [Inject]
        public void Construct(AssetCacheSystem assetCacheSystem)
        {
            this.assetCacheSystem = assetCacheSystem;

            foreach (var t in testData)
            {
                string path = $"/{t.filename}";

                loaderState.assetIds.Add(t.id);

                assetCacheSystem.Add(
                    t.id,
                    new MetadataFileFormat(t.id,
                        $"{path}.dclasset",
                        new MetadataFileFormat.Contents(
                            new MetadataFileFormat.AssetInfo(t.id, t.filename, t.filename, t.type),
                            ""
                )));

                assetCacheSystem.Add(
                    t.id,
                    new GltfFileFormat(t.id, path));
            }
        }

        public MockAssetLoader(params TestData[] testData)
        {
            this.testData = testData;
        }

        public void ClearAllData() => allAssetsCached = false;

        public void CacheAllAssetMetadata()
        {
            allAssetsCached = true;
        }

        public AssetHierarchyItem GetHierarchy()
        {
            return new AssetHierarchyItem();
        }

        public IEnumerable<Guid> GetAllAssetIds() => loaderState.assetIds;

        public Task<string> CopyAssetTo(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
