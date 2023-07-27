using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.System
{
    /// <summary>
    /// System to handle topics related to the application itself.
    /// </summary>
    public class ApplicationSystem
    {
        // Dependencies
        private SettingsSystem settingsSystem;
        private EditorEvents editorEvents;
        private MenuBarSystem menuBarSystem;
        private PromptSystem promptSystem;

        [Inject]
        private void Construct(
            SettingsSystem settingsSystem,
            EditorEvents editorEvents,
            MenuBarSystem menuBarSystem,
            PromptSystem promptSystem)
        {
            this.settingsSystem = settingsSystem;
            this.editorEvents = editorEvents;
            this.menuBarSystem = menuBarSystem;
            this.promptSystem = promptSystem;

            CreateMenuBarItems();
            CreateHelpMenuBarItems();

            // subscribe events
            this.editorEvents.onSettingsChangedEvent += SetApplicationTargetFramerate;
        }

        private void CreateMenuBarItems()
        {
            menuBarSystem.AddMenuItem("File#1/Exit#1000", QuitApplication);
        }

        private void CreateHelpMenuBarItems()
        {
            menuBarSystem.AddMenuItem("Help#30/Support",
                () => { Application.OpenURL("https://discord.com/channels/843557607373275206/1033310423010775120"); });
            menuBarSystem.AddMenuItem("Help#30/Tweet about us",
                () =>
                {
                    Application.OpenURL(
                        "https://twitter.com/intent/tweet?text=Using%20%23DCLEdit%20by%20%40MGH_DAO%20right%20now");
                });

            menuBarSystem.AddMenuItem("Help#30/Show Log",
                async () => { await promptSystem.CreateChangeLog(); });
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
            Application.targetFrameRate = settingsSystem.applicationTargetFramerate.Get();
        }
        
        /// <summary>
        /// Close the application
        /// </summary>
        private void QuitApplication()
        {
            //TODO: Check for unsaved content
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
