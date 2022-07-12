#if UNITY_EDITOR

using Assets.Scripts.EditorState;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Visuals
{
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

            //GUILayout.Button("hi");

            GUILayout.Label("Command History");
            GUILayout.Space(20);

            for (var i = 0; i < EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CommandHistory.Count; i++)
            {
                var command = EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CommandHistory[i];

                if (i == EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CurrentCommandIndex)
                    GUILayout.Label("Current: vvvvv");

                GUILayout.Label(command.Name);
                GUILayout.Label("    " + command.Description);

                if (i == EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CurrentCommandIndex)
                    GUILayout.Label("         ^^^^^");
            }

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
        }
    }
}

#endif