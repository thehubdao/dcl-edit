#if UNITY_EDITOR

using Assets.Scripts.EditorState;
using UnityEngine;
using UnityEditor;

public class DclUtilityWindow : EditorWindow
{

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/dcl-edit Utilities")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DclUtilityWindow window = (DclUtilityWindow)EditorWindow.GetWindow(typeof(DclUtilityWindow));
        window.Show();
    }

    private Vector2 _selectionScrollPosition = Vector2.zero;
    private Vector2 _buttonsScrollPosition = Vector2.zero;

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("dlc-edit is not running");
            return;
        }

        var selectionState = EditorStates.CurrentSceneState?.CurrentScene?.SelectionState;

        if (selectionState == null)
        {
            GUILayout.Label("No Scene currently loaded");
            return;
        }
        GUILayout.BeginHorizontal();

        _selectionScrollPosition = GUILayout.BeginScrollView(_selectionScrollPosition);

        GUILayout.Label("Primary Selection:");
        if (selectionState.PrimarySelectedEntity == null)
        {
            GUILayout.Label("    None");
        }
        else
        {
            CustomEditorUtils.DrawEntityToGui(selectionState.PrimarySelectedEntity, 1);
        }

        GUILayout.Label($"Secondary Selection: ({selectionState.SecondarySelectedEntities.Count})");
        foreach (var entity in selectionState.SecondarySelectedEntities)
        {
            CustomEditorUtils.DrawEntityToGui(entity, 1);
        }

        GUILayout.EndScrollView();

        _buttonsScrollPosition = GUILayout.BeginScrollView(_buttonsScrollPosition);

        GUILayout.Button("hi");

        GUILayout.EndScrollView();

        GUILayout.EndHorizontal();
    }
}

#endif