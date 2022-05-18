using System;
using Assets.Scripts.State;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class SceneState : MonoBehaviour
    {
        /**
         * <summary>
         * The scene path of the currently opened scene.
         * If empty string, no scene is opened
         * </summary>
         */
        [NonSerialized]
        private string _openedScenePath;
        public string OpenedScenePath => _openedScenePath;

        
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
            return !string.IsNullOrEmpty(_openedScenePath);
        }

        /**
         * <summary>
         * The currently opened scene object
         * </summary>
         */
        [NonSerialized]
        [CanBeNull]
        public DclScene CurrentScene;


    }
}
