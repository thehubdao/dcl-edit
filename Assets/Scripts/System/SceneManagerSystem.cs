using System;
using System.IO;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;
using Exception = System.Exception;

namespace Assets.Scripts.System
{
    public class SceneManagerSystem
    {
        // Dependencies
        private SceneManagerState sceneManagerState;
        private PathState pathState;
        private ISceneLoadSystem sceneLoadSystem;

        [Inject]
        public void Construct(
            SceneManagerState sceneManagerState,
            PathState pathState,
            ISceneLoadSystem sceneLoadSystem)
        {
            this.sceneManagerState = sceneManagerState;
            this.pathState = pathState;
            this.sceneLoadSystem = sceneLoadSystem;
        }

        public void DiscoverScenes()
        {
            // TODO: This should go through the asset manager

            var sceneDirectoryPaths = Directory.GetDirectories(pathState.ProjectPath, "*.dclscene", SearchOption.AllDirectories);

            foreach (var path in sceneDirectoryPaths)
            {
                var sds = LoadSceneDirectoryState(path);

                sceneManagerState.AddSceneDirectoryState(sds);
            }
        }

        public void SetFirstSceneAsCurrent()
        {
            sceneManagerState.SetCurrentSceneIndex(sceneManagerState.allSceneDirectoryStates.First().id);
        }

        public void SaveCurrentScene()
        {
            SaveScene(GetCurrentDirectoryState());
        }

        public void SaveScene(Guid id)
        {
            SaveScene(sceneManagerState.GetDirectoryState(id));
        }

        private void SaveScene(SceneDirectoryState sceneDirectoryState)
        {
            sceneSaveSystem.Save(sceneDirectoryState);
            workspaceSaveSystem.Save(); // TODO: Save the workspace under proper conditions.
            typeScriptGenerationSystem.GenerateTypeScript();
        }

        [CanBeNull]
        public DclScene GetCurrentScene()
        {
            //return null if no scene is open
            if (sceneManagerState.currentSceneIndex == Guid.Empty)
            {
                return null;
            }

            return GetScene(sceneManagerState.currentSceneIndex);
        }

        [NotNull]
        public DclScene GetScene(Guid index)
        {
            var sceneDirectoryState = sceneManagerState.GetDirectoryState(index);

            if (!sceneDirectoryState.IsSceneOpened())
            {
                sceneLoadSystem.Load(sceneDirectoryState);
            }

            return sceneDirectoryState.currentScene!;
        }

        private struct SceneFileContents
        {
            public Guid id;
            public string relativePath;
            public JObject settings;
        }

        public SceneDirectoryState LoadSceneDirectoryState(string path)
        {
            var sceneFileJson = Path.Combine(path, "scene.json");

            SceneFileContents sceneFileContents;
            try
            {
                sceneFileContents = JsonConvert.DeserializeObject<SceneFileContents>(sceneFileJson);
            }
            catch (Exception)
            {
                sceneFileContents = new SceneFileContents
                {
                    id = Guid.Empty,
                    relativePath = path,
                    settings = new JObject()
                };
            }

            if (sceneFileContents.id == Guid.Empty)
            {
                sceneFileContents.id = Guid.NewGuid();
            }

            return new SceneDirectoryState(sceneFileContents.relativePath, sceneFileContents.id);
        }

        public SceneDirectoryState GetCurrentDirectoryState()
        {
            return sceneManagerState.GetCurrentDirectoryState();
        }
    }
}