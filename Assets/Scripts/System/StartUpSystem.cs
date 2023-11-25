using Assets.Scripts.EditorState;
using System;
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
            AssetDiscovery assetDiscovery)
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
        }

        void Start()
        {
            workspaceSaveSystem.Load();

            frameTimeSystem.SetApplicationTargetFramerate();
        }
    }
}