using Assets.Scripts.EditorState;
using System;
using System.Collections;
using Assets.Scripts.Assets;
using Assets.Scripts.Events;
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
        private AvailableComponentsState availableComponentsState;
        private SceneJsonReaderSystem sceneJsonReaderSystem;
        private EntityPresetState entityPresetState;
        private CustomComponentDefinitionSystem customComponentDefinitionSystem;
        private FileUpgraderSystem fileUpgraderSystem;
        private AssetDiscovery assetDiscovery;
        private AssetFormatTransformer assetFormatTransformer;
        private DiscoveredAssets discoveredAssets;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            ApplicationSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            CustomComponentMarkupSystem customComponentMarkupSystem,
            AvailableComponentsState availableComponentsState,
            SceneJsonReaderSystem sceneJsonReaderSystem,
            EntityPresetState entityPresetState,
            CustomComponentDefinitionSystem customComponentDefinitionSystem,
            FileUpgraderSystem fileUpgraderSystem,
            AssetDiscovery assetDiscovery,
            AssetFormatTransformer assetFormatTransformer,
            DiscoveredAssets discoveredAssets)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.customComponentMarkupSystem = customComponentMarkupSystem;
            this.availableComponentsState = availableComponentsState;
            this.sceneJsonReaderSystem = sceneJsonReaderSystem;
            this.entityPresetState = entityPresetState;
            this.customComponentDefinitionSystem = customComponentDefinitionSystem;
            this.fileUpgraderSystem = fileUpgraderSystem;
            this.assetDiscovery = assetDiscovery;
            this.assetFormatTransformer = assetFormatTransformer;
            this.discoveredAssets = discoveredAssets;
        }

        void Awake()
        {
            fileUpgraderSystem.InitUpgrader();

            if (sceneJsonReaderSystem.IsEcs7())
            {
                availableComponentsState.AddEcs7BuildInComponents();
                entityPresetState.FillEcs7BuildInPresets();
            }
            else
            {
                availableComponentsState.AddEcs6BuildInComponents();
                entityPresetState.FillEcs6BuildInPresets();
            }

            //assetManagerSystem.CacheAllAssetMetadata();
            assetDiscovery.Initialize();
            assetFormatTransformer.Init();

            if (sceneJsonReaderSystem.IsEcs7())
            {
                customComponentDefinitionSystem.DiscoverComponentDefinitions();
            }
            else
            {
                customComponentMarkupSystem.SetupCustomComponents();
            }

            sceneManagerSystem.DiscoverScenes();

            sceneManagerSystem.SetLastOpenedSceneAsCurrentScene();

            StartCoroutine(LateAwake());
        }

        void Start()
        {
            workspaceSaveSystem.Load();

            frameTimeSystem.SetApplicationTargetFramerate();
        }

        IEnumerator LateAwake()
        {
            yield return new WaitForSeconds(1);

            var id = Guid.Parse("8d8d5f8b-2bd3-4da1-942d-d89c182ca020");

            discoveredAssets.discoveredAssets[id].assetFormatChanged += () => { Debug.Log(discoveredAssets.GetAssetFormat<AssetFormatBuilderDownload>(id)); };

            Debug.Log(discoveredAssets.GetAssetFormat<AssetFormatBuilderDownload>(id));
        }
    }
}