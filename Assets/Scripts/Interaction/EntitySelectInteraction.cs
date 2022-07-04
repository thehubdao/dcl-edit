using System;
using System.Linq;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Assets.Scripts.Interaction
{
    public class EntitySelectInteraction : MonoBehaviour
    {
        public Guid Id;

        public void SelectAdditional()
        {
            var scene = EditorStates.CurrentSceneState.CurrentScene;
            var selectionCommand = new ChangeSelection(
                ChangeSelection.GetPrimarySelectionFromScene(scene),
                ChangeSelection.GetSecondarySelectionFromScene(scene),
                Id,
                scene.SelectionState.AllSelectedEntities
                    .Select(e => e?.Id ?? Guid.Empty)
                    .Where(id => id != Guid.Empty && id != Id));

            CommandSystem.ExecuteCommand(selectionCommand);
        }

        public void SelectSingle()
        {
            var scene = EditorStates.CurrentSceneState.CurrentScene;
            var selectionCommand = new ChangeSelection(
                ChangeSelection.GetPrimarySelectionFromScene(scene),
                ChangeSelection.GetSecondarySelectionFromScene(scene),
                Id,
                Array.Empty<Guid>());

            CommandSystem.ExecuteCommand(selectionCommand);
        }
    }

    // Custom editor

#if UNITY_EDITOR
    
    [CustomEditor(typeof(EntitySelectInteraction))]
    //[CanEditMultipleObjects]
    public class LookAtPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var entitySelectInteraction = target as EntitySelectInteraction;

            if (entitySelectInteraction == null)
                return;

            if (GUILayout.Button("Select Additional"))
            {
                entitySelectInteraction.SelectAdditional();
            }

            if (GUILayout.Button("Select Single"))
            {
                entitySelectInteraction.SelectSingle();
            }
        }
    }

#endif

}