using System;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine;


namespace Assets.Scripts.EditorState
{
    public class SceneDirectoryState
    {
        /**
         * <summary>
         * The Path of the scene directory
         * </summary>
         */
        public string DirectoryPath;

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
            return CurrentScene != null;
        }

        /**
         * <summary>
         * The currently opened scene object
         * </summary>
         */
        [CanBeNull]
        public DclScene CurrentScene;

        /**
         * <summary>
         * Stores the paths of all files, that contribute to the opened scene. When the scene is saved again, it will only overwrite those files, that are part of the scene.
         * All files, that are in the scene directory but for what ever reason not part of the scene, will stay unharmed.
         * </summary>
         */
        public List<string> LoadedFilePathsInScene = new List<string>();
    }
}