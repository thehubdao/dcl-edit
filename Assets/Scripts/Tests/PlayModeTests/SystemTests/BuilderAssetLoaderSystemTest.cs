using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.PlayModeTests
{
    public class BuilderAssetLoaderSystemTest
    {
        // Dependencies
        private SceneDirectoryState sceneDirectoryState;
        private System.BuilderAssetLoaderSystem builderAssetLoaderSystem;
        private BuilderAssetLoaderState builderAssetLoaderState;
        private EditorEvents editorEvents;
        private System.LoadGltfFromFileSystem loadGltfSystem;
        private System.IWebRequestSystem webRequestSystem;
        private IPathState pathState;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            sceneDirectoryState = new SceneDirectoryState();
            builderAssetLoaderSystem = new System.BuilderAssetLoaderSystem();
            builderAssetLoaderState = new BuilderAssetLoaderState();
            editorEvents = new EditorEvents();
            loadGltfSystem = new System.LoadGltfFromFileSystem();
            webRequestSystem = new System.WebRequestSystem();
            pathState = new PathState();
        }

        private IEnumerator SetupScene()
        {
            SceneManager.LoadScene(0);
            yield return null;
            // create new dcl scene
            sceneDirectoryState.currentScene = new DclScene();
        }

        [UnityTest]
        public IEnumerator GetAssetThumbnail()
        {
            yield return SetupScene();

            bool metadataCacheUpdated = false;
            bool thumbnailCacheUpdated = false;
            editorEvents.onAssetMetadataCacheUpdatedEvent += () => metadataCacheUpdated = true;
            editorEvents.onAssetThumbnailUpdatedEvent += (_) => thumbnailCacheUpdated = true;

            // Get Unity State
            loadGltfSystem.Construct(UnityEngine.Object.FindObjectOfType<UnityState>());
            builderAssetLoaderSystem.Construct(builderAssetLoaderState, editorEvents, loadGltfSystem, webRequestSystem, pathState);

            builderAssetLoaderSystem.CacheAllAssetMetadata();

            while (metadataCacheUpdated == false) yield return null;

            var thumbnail1 = builderAssetLoaderSystem.GetThumbnailById(Guid.Parse("0149cae5-9e33-48aa-a346-94f02091ec75"));
            Assert.NotNull(thumbnail1);
            Assert.AreEqual(AssetData.State.IsLoading, thumbnail1.state);
            Assert.Null(thumbnail1.texture);

            while (thumbnailCacheUpdated == false) yield return null;

            var thumbnail2 = builderAssetLoaderSystem.GetThumbnailById(Guid.Parse("0149cae5-9e33-48aa-a346-94f02091ec75"));
            Assert.NotNull(thumbnail2);
            Assert.AreEqual(AssetData.State.IsAvailable, thumbnail2.state);
            Assert.NotNull(thumbnail2.texture);

            Assert.AreEqual(512, thumbnail2.texture.width);
            Assert.AreEqual(512, thumbnail2.texture.height);
        }
    }
}