using Assets.Scripts.EditorState;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zenject;
using static Assets.Scripts.System.PromptSystem;

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
        private PromptSystem promptSystem;

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
            PromptSystem promptSystem)
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
            this.promptSystem = promptSystem;
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
                Task<PromptAction> task = promptSystem.CreateDialog(
                    "DCL-Edit V3 does not support SDK6. Please upgrade your project to SDK7 or downgrade dcl-edit",
                    new PromptAction[] { new OK(() => Application.Quit()) },
                    new NotInWindow(() => Application.Quit()));

                return;
            }

            assetManagerSystem.CacheAllAssetMetadata();

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