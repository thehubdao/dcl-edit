using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class BuilderAssetLoaderSystemTest
    {
        [UnityTest]
        public IEnumerator TestWithNetworkConnection()
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


            var testObjectId = Guid.Parse("ab743f36-176b-4e74-897e-19e28cc6e425");

            Assert.Contains(testObjectId, enumerable);


            var testObjectMetaData = loaderSystem.GetMetadataById(testObjectId);

            Assert.AreEqual("Signpost Tree", testObjectMetaData.assetDisplayName);


            var testObjectData = loaderSystem.GetDataById(testObjectId);

            Assert.IsNotNull(testObjectData);
            Assert.AreEqual(AssetData.State.IsLoading, testObjectData.state);

            yield return assetDataUpdatedEvent.WaitForActionCount(1);

            var eventParams = assetDataUpdatedEvent.LastParam<List<Guid>>();
            var eventParam = eventParams.First();

            Assert.IsNotNull(eventParam);
            Assert.AreEqual(testObjectId, eventParam);

            testObjectData = loaderSystem.GetDataById(testObjectId);

            Assert.IsInstanceOf<ModelAssetData>(testObjectData);

            var testObjectModelData = testObjectData as ModelAssetData;

            Assert.IsNotNull(testObjectModelData);

            var testObjectObject = testObjectModelData.data;
            testObjectObject.SetActive(true);
            testObjectObject.transform.position = Vector3.zero;

            yield return WaitForSeconds.Wait(5);

            var testObjectThumbnailData = loaderSystem.GetThumbnailById(testObjectId);

            Assert.IsNotNull(testObjectThumbnailData);
        }

        [UnityTest]
        public IEnumerator GetAssetIds()
        {
            var loaderSystem = new BuilderAssetLoaderSystem();
            var loaderState = new BuilderAssetLoaderState();
            var editorEvent = new EditorEvents();
            var loadGltf = new LoadGltfFromFileSystem();
            var webRequest = new MockWebRequestSystem();

            // Get Unity State
            loadGltf.Construct(Object.FindObjectOfType<UnityState>());

            var assetCachedAction = new MockEventActionListener();
            editorEvent.onAssetMetadataCacheUpdatedEvent += assetCachedAction.Called;

            loaderSystem.Construct(loaderState, editorEvent, loadGltf, webRequest);

            loaderSystem.CacheAllAssetMetadata();

            yield return assetCachedAction.WaitForActionCount(1);

            var ids = loaderSystem.GetAllAssetIds();

            Assert.IsNotNull(ids);

            var idArray = ids as Guid[] ?? ids.ToArray();

            // Check that we have the correct assets
            Assert.AreEqual(10, idArray.Length);
            Assert.Contains(Guid.Parse("9bcc6035-2fe6-4588-88d6-c06e6d4e9c45"), idArray);
            Assert.Contains(Guid.Parse("7f22937e-cb5a-4f9a-99ac-08be17e58c37"), idArray);
            Assert.Contains(Guid.Parse("f6b0a279-3f7b-43d1-9382-08ad207ad706"), idArray);
            Assert.Contains(Guid.Parse("7cd62cdf-da5b-4230-83e2-b09b0c457c65"), idArray);

            yield break;
        }

        [UnityTest]
        public IEnumerator GetAssetMetaData()
        {
            var loaderSystem = new BuilderAssetLoaderSystem();
            var loaderState = new BuilderAssetLoaderState();
            var editorEvent = new EditorEvents();
            var loadGltf = new LoadGltfFromFileSystem();
            var webRequest = new MockWebRequestSystem();

            // Get Unity State
            loadGltf.Construct(Object.FindObjectOfType<UnityState>());

            var assetCachedAction = new MockEventActionListener();
            editorEvent.onAssetMetadataCacheUpdatedEvent += assetCachedAction.Called;

            loaderSystem.Construct(loaderState, editorEvent, loadGltf, webRequest);

            loaderSystem.CacheAllAssetMetadata();

            yield return assetCachedAction.WaitForActionCount(1);

            var metaData1 = loaderSystem.GetMetadataById(Guid.Parse("9bcc6035-2fe6-4588-88d6-c06e6d4e9c45"));

            Assert.IsNotNull(metaData1);
            Assert.AreEqual("Rustic Rope Bridge", metaData1.assetDisplayName);
            Assert.AreEqual(Guid.Parse("9bcc6035-2fe6-4588-88d6-c06e6d4e9c45"), metaData1.assetId);
            Assert.AreEqual(AssetMetadata.AssetType.Model, metaData1.assetType);


            var metaData2 = loaderSystem.GetMetadataById(Guid.Parse("0149cae5-9e33-48aa-a346-94f02091ec75"));

            Assert.IsNotNull(metaData2);
            Assert.AreEqual("Bazaar Tent", metaData2.assetDisplayName);
            Assert.AreEqual(Guid.Parse("0149cae5-9e33-48aa-a346-94f02091ec75"), metaData2.assetId);
            Assert.AreEqual(AssetMetadata.AssetType.Model, metaData2.assetType);
        }


        [UnityTest]
        public IEnumerator GetAssetData()
        {
            var loaderSystem = new BuilderAssetLoaderSystem();
            var loaderState = new BuilderAssetLoaderState();
            var editorEvent = new EditorEvents();
            var loadGltf = new LoadGltfFromFileSystem();
            var webRequest = new MockWebRequestSystem();

            // Get Unity State
            loadGltf.Construct(Object.FindObjectOfType<UnityState>());

            var assetCachedAction = new MockEventActionListener();
            editorEvent.onAssetMetadataCacheUpdatedEvent += assetCachedAction.Called;

            loaderSystem.Construct(loaderState, editorEvent, loadGltf, webRequest);

            loaderSystem.CacheAllAssetMetadata();

            yield return assetCachedAction.WaitForActionCount(1);

            var assetData1 = loaderSystem.GetDataById(Guid.Parse("9bcc6035-2fe6-4588-88d6-c06e6d4e9c45"));

            Assert.AreEqual(AssetData.State.IsLoading, assetData1.state);
        }
    }
}
