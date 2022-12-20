using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Assets.Scripts.System
{
    /// <summary>
    /// System to handle topics related to global timing and framerates.
    /// </summary>
    public class FrameTimeSystem
    {
        // Dependencies
        private SettingsSystem _settingsSystem;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(
            SettingsSystem settingsSystem,
            EditorEvents editorEvents)
        {
            _settingsSystem = settingsSystem;
            _editorEvents = editorEvents;

            // subscribe events
            _editorEvents.onSettingsChangedEvent += SetApplicationTargetFramerate;
        }

        
        /// <summary>
        /// Specifies the frame rate at which Unity tries to render your game.
        /// </summary>
        /// <param name="targetFramerate">
        /// A posive intger represening the target framerate in frames per second.
        /// The default value is -1, then Unity uses the platform's default target frame rate.
        /// </param>
        public void SetApplicationTargetFramerate()
        {
            Application.targetFrameRate = _settingsSystem.applicationTargetFrameRate.Get();
        }
    }

}