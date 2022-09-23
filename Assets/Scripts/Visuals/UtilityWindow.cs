#if UNITY_EDITOR

using System.Collections;
using Assets.Scripts.EditorState;
using Assets.Scripts.Interaction;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEditor;
using UnityEngine;
using Zenject;

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

        //private bool _showSelectionState = true;
        //private bool _showCommandHistory = true;

        // Dependencies
        private InputState _inputState;
        private UnityState _unityState;

        [Inject]
        private void Construct(InputState inputState, UnityState unityState)
        {
            _inputState = inputState;
            _unityState = unityState;
        }

        void OnGUI()
        {
            //if (!Application.isPlaying)
            //{
            //    GUILayout.Label("dlc-edit is not running");
            //    return;
            //}
            //
            //GUILayout.Label("InState: " + _inputState.InState);
            //
            //_showSelectionState = GUILayout.Toggle(_showSelectionState, "Show Selection State");
            //_showCommandHistory = GUILayout.Toggle(_showCommandHistory, "Show Command History");
            //
            //GUILayout.BeginHorizontal();
            //
            //if (_showSelectionState)
            //    DrawSelectionSection();
            //
            //if (_showCommandHistory)
            //    DrawCommandHistory();
            //
            //
            //GUILayout.EndHorizontal();
        }

        private void DrawSelectionSection()
        {
            //var selectionState = EditorStates.CurrentSceneState?.CurrentScene?.SelectionState;
            //
            //if (selectionState == null)
            //{
            //    GUILayout.Label("No Scene currently loaded");
            //    return;
            //}
            //
            //var selectedEntityVisual = Selection.activeGameObject?.GetComponentInParent<EntityVisuals>();
            //
            //
            //_selectionScrollPosition = GUILayout.BeginScrollView(_selectionScrollPosition);
            //
            //if (GUILayout.Button("dcl-edit -> Unity (Visual Representation)") && selectionState.PrimarySelectedEntity != null)
            //{
            //    Selection.activeGameObject =
            //        EditorStates.CurrentUnityState.SceneVisuals
            //            .GetComponentsInChildren<EntityVisuals>()
            //            .FirstOrNull(visuals => visuals.Id == selectionState.PrimarySelectedEntity.Id)
            //            .gameObject;
            //}
            //
            //if (GUILayout.Button("Unity -> dcl-edit") && selectedEntityVisual != null)
            //{
            //    selectedEntityVisual.GetComponent<EntitySelectInteraction>().SelectSingle();
            //    selectedEntityVisual.GetComponentInParent<MainSceneVisuals>().StartCoroutine(ReselectUnity(selectionState));
            //
            //}
            //
            //if (selectedEntityVisual != null)
            //{
            //    GUILayout.Label("Unity Selection:");
            //    CustomEditorUtils.DrawEntityToGui(
            //        EditorStates.CurrentSceneState.CurrentScene
            //            .GetEntityFormId(selectedEntityVisual.Id), 1, true);
            //}
            //
            //GUILayout.Label("Primary Selection:");
            //if (selectionState.PrimarySelectedEntity == null)
            //{
            //    GUILayout.Label("    None");
            //}
            //else
            //{
            //    CustomEditorUtils.DrawEntityToGui(selectionState.PrimarySelectedEntity, 1, true);
            //}
            //
            //GUILayout.Label($"Secondary Selection: ({selectionState.SecondarySelectedEntities.Count})");
            //foreach (var entity in selectionState.SecondarySelectedEntities)
            //{
            //    CustomEditorUtils.DrawEntityToGui(entity, 1, true);
            //}
            //
            //GUILayout.EndScrollView();
        }


        private void DrawCommandHistory()
        {
            //_buttonsScrollPosition = GUILayout.BeginScrollView(_buttonsScrollPosition);
            //
            //
            //GUILayout.Label("Command History");
            //GUILayout.Space(20);
            //
            //for (var i = 0; i < EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CommandHistory.Count; i++)
            //{
            //    var command = EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CommandHistory[i];
            //
            //    if (i == EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CurrentCommandIndex)
            //        GUILayout.Label("Current: vvvvv");
            //
            //    GUILayout.Label(command.Name);
            //    GUILayout.Label("    " + command.Description);
            //
            //    if (i == EditorStates.CurrentSceneState.CurrentScene.CommandHistoryState.CurrentCommandIndex)
            //        GUILayout.Label("         ^^^^^");
            //}
            //
            //GUILayout.EndScrollView();
        }
        //private IEnumerator ReselectUnity(SelectionState selectionState)
        //{
        //yield return null;
        //Selection.activeGameObject =
        //    EditorStates.CurrentUnityState.SceneVisuals
        //        .GetComponentsInChildren<EntityVisuals>()
        //        .FirstOrNull(visuals => visuals.Id == selectionState.PrimarySelectedEntity.Id)
        //        .gameObject;
        //}
    }
}

#endif