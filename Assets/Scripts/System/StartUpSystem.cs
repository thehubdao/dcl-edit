using Assets.Scripts.EditorState;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        [SerializeField]
        private CameraSystem _cameraSystem;

        // dependencies
        private AssetManagerSystem assetManagerSystem;
        private WorkspaceSaveSystem workspaceSaveSystem;
        private ApplicationSystem frameTimeSystem;
        private SceneManagerSystem sceneManagerSystem;
        private CustomComponentMarkupSystem customComponentMarkupSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            ApplicationSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            CustomComponentMarkupSystem customComponentMarkupSystem)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.customComponentMarkupSystem = customComponentMarkupSystem;
        }

        void Awake()
        {
            assetManagerSystem.CacheAllAssetMetadata();

            customComponentMarkupSystem.SetupCustomComponents();

            sceneManagerSystem.DiscoverScenes();

            sceneManagerSystem.SetLastOpenedSceneAsCurrentScene();
        }

        void Start()
        {
            workspaceSaveSystem.Load();

            frameTimeSystem.SetApplicationTargetFramerate();
        }
    }
}