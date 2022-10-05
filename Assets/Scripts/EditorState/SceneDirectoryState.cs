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

        public List<string> LoadedFilePathsInScene = new List<string>();
    }
}