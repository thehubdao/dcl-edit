using System;
using System.Linq;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class EntitySelectInteraction : MonoBehaviour
    {
        public Guid Id;

        // dependencies
        private ICommandSystem _commandSystem;
        private EditorState.SceneDirectoryState _sceneDirectoryState;

        [Inject]
        public void Construct(ICommandSystem commandSystem, EditorState.SceneDirectoryState sceneDirectoryState)
        {
            _commandSystem = commandSystem;
            _sceneDirectoryState = sceneDirectoryState;
        }

        public void SelectAdditional()
        {
            var scene = _sceneDirectoryState.CurrentScene;
            var selectionCommand = _commandSystem.CommandFactory.CreateChangeSelection(
                ChangeSelection.GetPrimarySelectionFromScene(scene),
                ChangeSelection.GetSecondarySelectionFromScene(scene),
                Id,
                scene.SelectionState.AllSelectedEntities
                    .Select(e => e?.Id ?? Guid.Empty)
                    .Where(id => id != Guid.Empty && id != Id));

            _commandSystem.ExecuteCommand(selectionCommand);
        }

        public void SelectSingle()
        {
            var scene = _sceneDirectoryState.CurrentScene;
            var selectionCommand = _commandSystem.CommandFactory.CreateChangeSelection(
                ChangeSelection.GetPrimarySelectionFromScene(scene),
                ChangeSelection.GetSecondarySelectionFromScene(scene),
                Id,
                Array.Empty<Guid>());

            _commandSystem.ExecuteCommand(selectionCommand);
        }

        public class Factory : PlaceholderFactory<EntitySelectInteraction>
        {
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