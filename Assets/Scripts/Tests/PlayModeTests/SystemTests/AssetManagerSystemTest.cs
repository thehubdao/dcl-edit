using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace Assets.Scripts.Tests.PlayModeTests
{
    public class AssetManagerSystemTest : ZenjectIntegrationTestFixture
    {
        Guid model1Id;
        Guid model2Id;
        Guid model3Id;

        void CommonInstall()
        {
            // Setup initial state by creating game objects from scratch, loading prefabs/scenes, etc
            PreInstall();

            model1Id = Guid.Parse("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
            model2Id = Guid.Parse("77238038-082b-4ddc-8cfe-657042bb229e");
            model3Id = Guid.Parse("4f7a3e35-7b33-47da-bce6-087b5ca30d2e");


            Container.BindInterfacesAndSelfTo<EditorEvents>().AsSingle();
            Container.BindFactory<Guid, GameObject, LoadedModelFormat, LoadedModelFormat.Factory>().AsSingle();
            Container.BindInterfacesAndSelfTo<AssetCacheSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<AssetManagerSystem>().AsSingle();
            Container.Bind<IAssetLoaderSystem>().To<MockAssetLoader>().AsSingle().WithArguments(new MockAssetLoader.TestData[]
            {
                new MockAssetLoader.TestData { filename = "model1.glb", type = AssetMetadata.AssetType.Model, id = model1Id },
                new MockAssetLoader.TestData { filename = "model2.glb", type = AssetMetadata.AssetType.Model, id = model2Id },
                new MockAssetLoader.TestData { filename = "model3.glb", type = AssetMetadata.AssetType.Model, id = model3Id }
            });

            // Call Container.Bind methods
            PostInstall();
        }


        [UnityTest]
        public IEnumerator TestGetMetadataById()
        {
            CommonInstall();

            var assetManagerSystem = Container.Resolve<AssetManagerSystem>();

            var metadata = assetManagerSystem.GetMetadataById(model1Id);
            Assert.NotNull(metadata);
            Assert.AreEqual("model1.glb", metadata.assetDisplayName);
            Assert.AreEqual(AssetMetadata.AssetType.Model, metadata.assetType);
            Assert.AreEqual(model1Id, metadata.assetId);

            yield break;
        }

        [UnityTest]
        public IEnumerator TestGetAllAssetIds()
        {
            CommonInstall();

            var assetManagerSystem = Container.Resolve<AssetManagerSystem>();

            var ids = assetManagerSystem.GetAllAssetIds();

            Assert.AreEqual(ids.Count(), 3);
            Assert.True(ids.Contains(model1Id));
            Assert.True(ids.Contains(model2Id));
            Assert.True(ids.Contains(model3Id));

            yield break;
        }
    }
}