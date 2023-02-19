using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;


namespace Assets.Scripts.EditorState
{
    public class SceneDirectoryState
    {
        /**
         * <summary>
         * The Path of the scene directory. This can be null and means that the scene was not saved before
         * </summary>
         */
        [CanBeNull]
        public string directoryPath = null;

        /**
         * <summary>
         * Checks, if any scene is currently opened
         * </summary>
         *
         * <returns>
         * True, if any scene is currently opened
         * </returns>
         */
        public bool IsSceneOpened()
        {
            return currentScene != null;
        }

        /**
         * <summary>
         * The currently opened scene object
         * </summary>
         */
        [CanBeNull]
        public DclScene currentScene = null;

        /**
         * <summary>
         * Stores the paths of all files, that contribute to the opened scene. When the scene is saved again, it will only overwrite those files, that are part of the scene.
         * All files, that are in the scene directory but for what ever reason not part of the scene, will stay unharmed.
         * </summary>
         */
        public List<string> loadedFilePathsInScene = new List<string>();

        /**
         * <summary>
         * The id of the scene
         * </summary>
         */
        public Guid id;

        public string name = "New Scene";

        public SceneDirectoryState()
        {
            id = Guid.NewGuid();
        }
        
        /**
         * <summary>
         * The version of the dcl-edit project
         * </summary>
         */
        public readonly DclEditVersion dclEditVersion = DclEditVersion.None;

        public SceneDirectoryState(string directoryPath, Guid id, DclEditVersion dclEditVersion = DclEditVersion.Beta)
        {
            this.directoryPath = directoryPath;
            this.id = id;
            this.dclEditVersion = dclEditVersion;
        }

        /// <summary>
        /// Creates a deep Copy. 
        /// </summary>
        /// <param name="id">This id will be assigned to the copy.</param>
        public SceneDirectoryState DeepCopy(Guid id)
        {
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
            }

            SceneDirectoryState copy = DeepCopy();
            copy.id = id;
            return copy;
        }

        /// <summary>
        /// Creates a deep Copy. 
        /// </summary>
        public SceneDirectoryState DeepCopy()
        {
            SceneDirectoryState copy = new SceneDirectoryState();
            copy.directoryPath = directoryPath;
            copy.currentScene = currentScene.DeepCopy();
            return copy;
        }

        /// <summary>
        /// Creates a new SceneDirectoryState and adds a Scene to it.
        /// </summary>
        public static SceneDirectoryState CreateNewSceneDirectoryState()
        {
            return new SceneDirectoryState { currentScene = new DclScene() };
        }
    }
    
    public enum DclEditVersion
    {
        None,
        Alpha,
        Beta
    }
}