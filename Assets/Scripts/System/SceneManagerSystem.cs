using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using USFB;
using Zenject;

namespace Assets.Scripts.System
{
    public class SceneManagerSystem
    {
        // Dependencies
        private SceneManagerState sceneManagerState;
        private IPathState pathState;
        private ISceneLoadSystem sceneLoadSystem;
        private ISceneSaveSystem sceneSaveSystem;
        private SceneSettingState sceneSettingState;
        private CheckVersionSystem checkVersionSystem;
        private WorkspaceSaveSystem workspaceSaveSystem;
        private TypeScriptGenerationSystem typeScriptGenerationSystem;
        private SceneViewSystem sceneViewSystem;
        private MenuBarSystem menuBarSystem;

        [Inject]
        public void Construct(
            SceneManagerState sceneManagerState,
            IPathState pathState,
            ISceneLoadSystem sceneLoadSystem,
            ISceneSaveSystem sceneSaveSystem,
            SceneSettingState sceneSettingState,
            CheckVersionSystem checkVersionSystem,
            WorkspaceSaveSystem workspaceSaveSystem,
            TypeScriptGenerationSystem typeScriptGenerationSystem,
            SceneViewSystem sceneViewSystem,
            MenuBarSystem menuBarSystem)
        {
            this.sceneManagerState = sceneManagerState;
            this.pathState = pathState;
            this.sceneLoadSystem = sceneLoadSystem;
            this.sceneSaveSystem = sceneSaveSystem;
            this.sceneSettingState = sceneSettingState;
            this.checkVersionSystem = checkVersionSystem;
            this.workspaceSaveSystem = workspaceSaveSystem;
            this.typeScriptGenerationSystem = typeScriptGenerationSystem;
            this.sceneViewSystem = sceneViewSystem;
            this.menuBarSystem = menuBarSystem;

            CreateMenuBarItems();
        }

        /// <summary>
        /// Checks for a decentraland scene and if it exists,
        /// adds the scene directory states of existing beta/alpha dcl-edit projects and
        /// creates a new scene directory state, if no dcl-edit project exists.  
        /// </summary>
        public void DiscoverScenes()
        {
            if (!checkVersionSystem.CheckDclSceneExists())
            {
                //TODO Display a message, that there is no project and exit after user input
                Debug.LogError("No Decentraland Folder found");
                return;
            }

            if (checkVersionSystem.TryGetBetaPaths(out var betaSceneDirectoryPaths))
            {
                foreach (var path in betaSceneDirectoryPaths)
                {
                    var sceneDirectoryState = LoadSceneDirectoryState(path);

                    if (!sceneManagerState.TryGetDirectoryState(sceneDirectoryState.directoryPath, out sceneDirectoryState))
                    {
                        sceneManagerState.AddSceneDirectoryState(sceneDirectoryState);
                    }
                }
            }
            //Check for dcl-edit alpha projects or missing project
            else
            {
                var doAlphaFilesExist = checkVersionSystem.CheckForAlpha();

                var sceneDirectoryState = doAlphaFilesExist
                    ? new SceneDirectoryState(null, Guid.NewGuid(), DclEditVersion.Alpha)
                    : SceneDirectoryState.CreateNewSceneDirectoryState();

                //Don't have to check for duplicates since non existing / alpha scenes get converted.
                sceneManagerState.AddSceneDirectoryState(sceneDirectoryState);
            }
            // TODO: This should go through the asset manager
        }

        /// <summary>
        /// Set any scene as current scene. Undefinded behaviour.
        /// </summary>
        public void SetFirstSceneAsCurrentScene()
        {
            SetCurrentScene(sceneManagerState.allSceneDirectoryStates.First().id);
        }

        /// <summary>
        /// Set last opened scene on start up
        /// </summary>
        public void SetLastOpenedSceneAsCurrentScene(SettingsSystem.StringUserSetting openLastOpenedScene)
        {
            Guid lastSceneIndex;

            if (Guid.TryParse(openLastOpenedScene.Get(), out lastSceneIndex))
            {
                var lastSceneDirectoryState = new SceneDirectoryState(null, lastSceneIndex, DclEditVersion.Alpha);
                sceneManagerState.AddSceneDirectoryState(lastSceneDirectoryState);
                //SaveCurrentScene();
                SaveScene(lastSceneDirectoryState);
                SetCurrentScene(lastSceneIndex);
                openLastOpenedScene.Set(lastSceneIndex.ToString());
            }
            else if (lastSceneIndex == Guid.Empty)
            {
                SetNewSceneAsCurrentScene();
                var newSceneIndex = sceneManagerState.currentSceneIndex;
                openLastOpenedScene.Set(newSceneIndex.ToString());
            }
        }

        /// <summary>
        /// Create a new scene and set it as current scene
        /// </summary>
        public void SetNewSceneAsCurrentScene()
        {
            SceneDirectoryState newScene = SceneDirectoryState.CreateNewSceneDirectoryState();
            sceneManagerState.AddSceneDirectoryState(newScene);
            SetCurrentScene(newScene.id);

        }

        /// <summary>
        /// Start an open file dialog and set the resulting scene as current scene.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SetDialogAsCurrentScene()
        {
            string[] paths = StandaloneFileBrowser.OpenFolderPanel("Open Scene", pathState.ProjectPath, false);

            // check for canceled dialog
            if (paths.Length == 0)
            {
                return;
            }

            SetCurrentScene(paths[0]);
        }

        /// <summary>
        /// Set the given scene as current scene.
        /// </summary>
        /// <param name="path">The scene to set as current scene.</param>
        public void SetCurrentScene(string path)
        {
            SetCurrentScene(sceneManagerState.GetDirectoryState(path).id);
        }

        /// <summary>
        /// Set the given scene as current scene.
        /// </summary>
        /// <param name="id">The ID of the scene to set as current scene.</param>
        public void SetCurrentScene(Guid id)
        {
            sceneManagerState.SetCurrentSceneIndex(id);
            sceneViewSystem.SetUpCurrentScene();
        }

        /// <summary>
        /// Save the current scene to its directory.
        /// If it has no directory yet, this will result in save as.
        /// Note: Save as might actually not save, if the user does cancel the dialog.
        /// </summary>
        public void SaveCurrentScene()
        {
            SaveScene(GetCurrentDirectoryState());
        }

        /// <summary>
        /// Save the given scene to its directory.
        /// If it has no directory yet, this will result in save as.
        /// Note: Save as might actually not save, if the user does cancel the dialog.
        /// </summary>
        /// <param name="id">The ID of the scene to save.</param>
        public void SaveScene(Guid id)
        {
            SaveScene(sceneManagerState.GetDirectoryState(id));
        }

        /// <summary>
        /// Save the given scene to its directory.
        /// If it has no directory yet, this will result in save as.
        /// Note: Save as might actually not save, if the user does cancel the dialog.
        /// </summary>
        /// <param name="sceneDirectoryState">The SceneDirectoryState of the scene to save.</param>
        private void SaveScene(SceneDirectoryState sceneDirectoryState)
        {
            // Use save as if scene has no path yet
            if (sceneDirectoryState.directoryPath == null)
            {
                SaveSceneAs(sceneDirectoryState);
            }
            else
            {
                sceneSaveSystem.Save(sceneDirectoryState);
                workspaceSaveSystem.Save(); // TODO: Save the workspace under proper conditions.
                sceneSettingState.SaveSettings();
                typeScriptGenerationSystem.GenerateTypeScript();
            }
        }

        /// <summary>
        /// Save the current scene to its directory with dialog.
        /// Note: This might actually not save, if the user does cancel the dialog.
        /// </summary>
        public void SaveCurrentSceneAs()
        {
            SaveSceneAs(GetCurrentDirectoryState());
        }

        /// <summary>
        /// Save the given scene to its directory with dialog.
        /// Note: This might actually not save, if the user does cancel the dialog.
        /// </summary>
        /// <param name="id">The SceneDirectoryState of the scene to save.</param>
        public void SaveSceneAs(Guid id)
        {
            SaveSceneAs(sceneManagerState.GetDirectoryState(id));
        }

        /// <summary>
        /// Save the given scene to its directory with dialog.
        /// Note: This might actually not save, if the user does cancel the dialog.
        /// </summary>
        /// <param name="sceneDirectoryState">The SceneDirectoryState of the scene to save.</param>
        private void SaveSceneAs(SceneDirectoryState sceneDirectoryState)
        {
            string oldPath = sceneDirectoryState.directoryPath;

            string oldContainingDirectoryPath;
            string oldName;
            if (oldPath == null)
            {
                oldContainingDirectoryPath = pathState.ProjectPath;
                oldName = "New Scene";
            }
            else
            {
                oldPath = Path.GetFullPath(oldPath); //normalize path format (e.g. turn '/' into '\\')
                oldContainingDirectoryPath = oldPath.Substring(0, oldPath.LastIndexOf(Path.DirectorySeparatorChar));
                oldName = oldPath.Substring(oldPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                oldName = oldName.Substring(0, oldName.LastIndexOf('.'));
            }

            string newPath = StandaloneFileBrowser.SaveFilePanel("Save Scene", oldContainingDirectoryPath, oldName, "dclscene");

            // check for canceled dialog
            if (newPath == "")
            {
                return;
            }

            //remove any potential scene that will be overridden
            if (sceneManagerState.TryGetDirectoryState(newPath, out SceneDirectoryState sceneDirectoryStateToOverride))
            {
                sceneManagerState.RemoveSceneDirectoryState(sceneDirectoryStateToOverride);
            }

            sceneDirectoryState.directoryPath = newPath;
            SaveScene(sceneDirectoryState);

            if (oldPath != null)
            {
                LoadSceneDirectoryState(oldPath);
            }
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
        public DclScene GetScene(Guid id)
        {
            var sceneDirectoryState = sceneManagerState.GetDirectoryState(id);

            if (!sceneDirectoryState.IsSceneOpened())
            {
                Load(id);
            }

            return sceneDirectoryState.currentScene!;
        }

        /// <summary>
        /// Loads the scene contents into an existing alpha or beta dcl-edit scene.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene</param>
        /// <exception cref="ArgumentException">Thrown when the scene directory state is neither 'alpha' nor 'beta'</exception>
        private void Load(Guid sceneIndex)
        {
            var sceneDirectoryState = sceneManagerState.GetDirectoryState(sceneIndex);

            if (sceneDirectoryState == null)
            {
                Debug.LogError("Scene directory state doesn't exist");
                return;
            }

            switch (sceneDirectoryState.dclEditVersion)
            {
                case DclEditVersion.Alpha:
                    sceneLoadSystem.LoadV1(sceneDirectoryState);
                    break;
                case DclEditVersion.Beta:
                    //TODO Implement a version check (i.e. 2.0)
                    //TODO Display a message, that the user should update dcl-edit and exit after user input 
                    sceneLoadSystem.Load(sceneDirectoryState);
                    break;
                default:
                    throw new ArgumentException("Scene directory state is neither alpha nor beta");
            }
        }

        /// <summary>
        /// Load a Scene from the file system and add it to the loaded SceneDirectoryStates in SceneManagerState.
        /// </summary>
        /// <param name="path">The directory to load the scene from.</param>
        /// <returns>The new loaded scene.</returns>
        private SceneDirectoryState LoadSceneDirectoryState(string path)
        {
            SceneDirectoryState sceneDirectoryState = new SceneDirectoryState(path, Guid.NewGuid());
            sceneLoadSystem.Load(sceneDirectoryState);
            sceneManagerState.AddSceneDirectoryState(sceneDirectoryState);
            return sceneDirectoryState;
        }

        public SceneDirectoryState GetCurrentDirectoryState()
        {
            return sceneManagerState.GetCurrentDirectoryState();
        }

        private void CreateMenuBarItems()
        {
            menuBarSystem.AddMenuItem("File#1/New Scene#1", SetNewSceneAsCurrentScene);
            menuBarSystem.AddMenuItem("File#1/Open Scene#2", SetDialogAsCurrentScene);
            menuBarSystem.AddMenuItem("File#1/Save Scene#3", SaveCurrentScene);
            menuBarSystem.AddMenuItem("File#1/Save Scene As...#4", SaveCurrentSceneAs);
        }

        public struct SceneFileContents
        {
            public Guid id;
            public string relativePath;
            public JObject settings;
            public DclEditVersion dclEditVersion;
        }
    }
}
