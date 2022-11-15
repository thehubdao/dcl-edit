using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class BuilderAssetLoaderSystemTest
    {
        [UnityTest]
        public IEnumerator CacheAssets()
        {
            var loaderSystem = new BuilderAssetLoaderSystem();
            var loaderState = new BuilderAssetLoaderState();
            var editorEvent = new EditorEvents();
            var loadGltf = new LoadGltfFromFileSystem();
            var webRequest = new WebRequestSystem();

            // Get Unity State
            loadGltf.Construct(Object.FindObjectOfType<UnityState>());

            var assetCachedAction = new MockEventActionListener();
            editorEvent.onAssetMetadataCacheUpdatedEvent += assetCachedAction.Called;

            var assetDataUpdatedEvent = new MockEventActionListener();
            editorEvent.onAssetDataUpdatedEvent += assetDataUpdatedEvent.Called;

            loaderSystem.Construct(loaderState, editorEvent, loadGltf, webRequest);

            loaderSystem.CacheAllAssetMetadata();

            yield return assetCachedAction.WaitForActionCount(1);

            var ids = loaderSystem.GetAllAssetIds();

            var enumerable = ids as Guid[] ?? ids.ToArray();

            Debug.Log(enumerable.Length);


            var signpostTreeId = Guid.Parse("ab743f36-176b-4e74-897e-19e28cc6e425");

            Assert.Contains(signpostTreeId, enumerable);


            var signpostTreeMetaData = loaderSystem.GetMetadataById(signpostTreeId);

            Assert.AreEqual("Signpost Tree", signpostTreeMetaData.assetDisplayName);


            var signpostTreeData = loaderSystem.GetDataById(signpostTreeId);

            Assert.IsNotNull(signpostTreeData);
            Assert.AreEqual(AssetData.State.IsLoading, signpostTreeData.state);

            yield return assetDataUpdatedEvent.WaitForActionCount(1);

            var eventParams = assetDataUpdatedEvent.LastParam<List<Guid>>();
            var eventParam = eventParams.First();

            Assert.IsNotNull(eventParam);
            Assert.AreEqual(signpostTreeId, eventParam);

            signpostTreeData = loaderSystem.GetDataById(signpostTreeId);

            Assert.IsInstanceOf<ModelAssetData>(signpostTreeData);

            var signpostTreeModelData = signpostTreeData as ModelAssetData;

            Assert.IsNotNull(signpostTreeModelData);
        }
    }
}
