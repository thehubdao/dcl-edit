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
        }
    }
}
