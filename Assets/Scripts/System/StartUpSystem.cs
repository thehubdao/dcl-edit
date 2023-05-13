using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;
using System;

namespace Assets.Scripts.System
{
    public class StartUpSystem : MonoBehaviour
    {
        [SerializeField]
        private CameraSystem _cameraSystem;

        // dependencies
        private AssetManagerSystem assetManagerSystem;
        private WorkspaceSaveSystem workspaceSaveSystem;
        private IPathState pathState;
        private ApplicationSystem frameTimeSystem;
        private SceneManagerSystem sceneManagerSystem;
        private SceneViewSystem sceneViewSystem;
        private SettingsSystem settingsSystem;
        private CustomComponentMarkupSystem customComponentMarkupSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            IPathState pathState,
            ApplicationSystem frameTimeSystem,
            SceneManagerSystem sceneManagerSystem,
            SceneViewSystem sceneViewSystem,
            SettingsSystem settingsSystem,
            CustomComponentMarkupSystem customComponentMarkupSystem)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.frameTimeSystem = frameTimeSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneViewSystem = sceneViewSystem;
            this.settingsSystem = settingsSystem;
            this.pathState = pathState;
            this.customComponentMarkupSystem = customComponentMarkupSystem;
        }

        void Awake()
        {
            InterpretArgs(Environment.GetCommandLineArgs());

            // assetManagerSystem.CacheAllAssetMetadata();

            customComponentMarkupSystem.SetupCustomComponents();

            sceneManagerSystem.DiscoverScenes();

            sceneManagerSystem.SetLastOpenedSceneAsCurrentScene();
        }

        void Start()
        {
            workspaceSaveSystem.Load();

            frameTimeSystem.SetApplicationTargetFramerate();
        }

        private void InterpretArgs(string[] args)
        {
            string projectPath = null;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "--projectPath")
                {
                    projectPath = args[i + 1];
                }
            }

            if (projectPath != null)
            {
                pathState.ProjectPath = projectPath;
            }
        }
    }
}