// Summary
//  Disables Editor Keyboard Behavior when Unity Player has focus, so your game can handle CTRL and 
//  without corrupting your scene.
//
// Usage
//  * Download or copy/paste and attach this component to a scene management object in your scene.
//  * Create a new shortcut profile using Edit->Shortcuts (tedious, you need to pretty much click every
//    button and remove every keystroke, one by one).
//
// Remarks
//   Editor preferences are stored in the Registry so there's no lightweight way to transfer them to
//   other members on your team. Pretty much, everyone needs to perform the tedious step of creating a
//   Player profile manually, and possibly for every install of Unity.
//
// Author: Jake Stine  (@MysteriousNixon)
// GIST URL: https://gist.github.com/jstine35/2a0b491cb1c1822debbc2571b487ad8d

using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class DisableEditorShortcutsOnGameFocus : MonoBehaviour
{
    [Tooltip("Create a playing profile using Edit->Shortcuts")]
    public string PlayingProfileId = "Playing";

    private static string LastEditorProfileId;

#if UNITY_EDITOR

    private static bool JustMadePlayingProfile = false;


    private static void SetActiveProfile(string profileId)
    {
        //Debug.Log($"Activating Shortcut profile '{profileId}'");
        
        ShortcutManager.instance.activeProfileId = profileId;

        if (JustMadePlayingProfile)
        {
            foreach(var id in ShortcutManager.instance.GetAvailableShortcutIds())
            {
                Debug.Log(id);
            }

            ShortcutManager.instance.RebindShortcut("Main Menu/Edit/Undo", ShortcutBinding.empty);
            ShortcutManager.instance.RebindShortcut("Main Menu/Edit/Redo", ShortcutBinding.empty);
            ShortcutManager.instance.RebindShortcut("Main Menu/File/Save", ShortcutBinding.empty);

            JustMadePlayingProfile = false;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        //Debug.Log("On Application Focus");

        var curProfileId = ShortcutManager.instance.activeProfileId;
        var newProfileId = LastEditorProfileId;

        if (hasFocus)
        {
            if (EditorApplication.isPaused)
            {
                return;
            }

            newProfileId = PlayingProfileId;

            if (string.IsNullOrEmpty(newProfileId))
            {
                Debug.Log($"PlayingProfileId is empty. Disable component SwapEditorShortcutsOnPlayerFocus to silence this warning.");
                return;
            }

            if (ShortcutManager.instance.GetAvailableProfileIds().All(s => s != newProfileId))
            {
                ShortcutManager.instance.CreateProfile(newProfileId);
                JustMadePlayingProfile = true;
            }

            if (curProfileId != PlayingProfileId)
            {
                LastEditorProfileId = curProfileId;
            }
        }
        else
        {
            newProfileId = LastEditorProfileId;
            LastEditorProfileId = null;
        }
        if (!string.IsNullOrEmpty(newProfileId))
        {
            SetActiveProfile(newProfileId);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
    }

    // statically bind Editor's Play/Pause button callbacks.
    // pauseStateChanged binding - this is useful to maintain 'expected behavior'. The dev typically wants all
    //   regular editor hotkeys to be effective any time the player is paused.
    //
    // playModeStateChanged binding - this is a safeguard / fallback to handle cases where Unity might 
    //   fail to send in-game  OnApplicationFocus() callbacks. I haven't specifically seen a case where it
    //   happens, but it's not unreasonable to think it could happen during some attached-debugging operations.

    static DisableEditorShortcutsOnGameFocus()
    {
        EditorApplication.playModeStateChanged += DetectEditorPlayModeState;
        EditorApplication.pauseStateChanged += DetectEditorPauseState;
    }

    private static void DetectEditorPauseState(PauseState state)
    {
        if (state == PauseState.Paused)
        {
            OnExitingPlayMode();
        }
    }

    private static void DetectEditorPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            OnExitingPlayMode();
        }
    }

    private static void OnExitingPlayMode()
    {
        if (!string.IsNullOrEmpty(LastEditorProfileId))
        {
            SetActiveProfile(LastEditorProfileId);
            LastEditorProfileId = null;
        }
    }
#endif
}