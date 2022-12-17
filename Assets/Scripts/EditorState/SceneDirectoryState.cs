using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine;


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

        public SceneDirectoryState()
        {
            id = Guid.NewGuid();
        }

        public SceneDirectoryState(string directoryPath, Guid id)
        {
            this.directoryPath = directoryPath;
        }
    }
}