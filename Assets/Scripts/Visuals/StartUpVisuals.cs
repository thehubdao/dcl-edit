using System.Threading.Tasks;
using Assets.Scripts.System;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Visuals
{
    public class StartUpVisuals: MonoBehaviour
    {
        // Dependencies
        private PromptSystem promptSystem;
        private MenuBarSystem menuBarSystem;
        private SceneChangeDetectSystem sceneChangeDetectSystem;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(PromptSystem promptSystem, MenuBarSystem menuBarSystem,
            SceneChangeDetectSystem sceneChangeDetectSystem, SceneManagerSystem sceneManagerSystem)
        {
            this.promptSystem = promptSystem;
            this.menuBarSystem = menuBarSystem;
            this.sceneChangeDetectSystem = sceneChangeDetectSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            
            CreateMenuBarItems();
            CreateHelpMenuBarItems();
            
            // Set on quit event
            Application.wantsToQuit += QuitApplication;
        }

        private void CreateMenuBarItems()
        {
            menuBarSystem.AddMenuItem("File#1/Exit#1000", () => QuitApplication());
        }
        
        private void CreateHelpMenuBarItems()
        {
            menuBarSystem.AddMenuItem("Help#30/Support",
                () =>
                {
                    Application.OpenURL("https://discord.com/channels/843557607373275206/1033310423010775120");
                });
            menuBarSystem.AddMenuItem("Help#30/Tweet about us",
                () =>
                {
                    Application.OpenURL(
                        "https://twitter.com/intent/tweet?text=Using%20%23DCLEdit%20by%20%40MGH_DAO%20right%20now");
                });
        }


        private bool forceQuit = false;

        /// <summary>
        /// Close the application
        /// </summary>
        private bool QuitApplication()
        {
            if (forceQuit)
                return true;

            if (!sceneChangeDetectSystem.HasSceneChanged())
                return true;

            var quitActions = new PromptSystem.PromptAction[]
            {
                new PromptSystem.Yes(() =>
                {
                    sceneManagerSystem.SaveCurrentScene();

#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }),
                new PromptSystem.No(() =>
                {
                    forceQuit = true;
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }),
                new PromptSystem.Cancel()
            };

#pragma warning disable CS4014 // async method is not awaited
            promptSystem.CreateDialog(
                "Do you want to save before quitting?",
                quitActions,
                new PromptSystem.NotInWindow()
            );
#pragma warning restore CS4014

            return false;
        }
    }
}