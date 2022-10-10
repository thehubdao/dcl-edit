using System;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Interaction
{
    public class EntitySelectInteraction : MonoBehaviour
    {
        public Guid Id;

        // dependencies
        private ICommandSystem _commandSystem;
        private EditorState.SceneDirectoryState _sceneDirectoryState;
        private EntitySelectSystem _entitySelectSystem;

        [Inject]
        public void Construct(ICommandSystem commandSystem, EditorState.SceneDirectoryState sceneDirectoryState, EntitySelectSystem entitySelectSystem)
        {
            _commandSystem = commandSystem;
            _sceneDirectoryState = sceneDirectoryState;
            _entitySelectSystem = entitySelectSystem;
        }

        public void Select()
        {
            _entitySelectSystem.ClickedOnEntity(Id);
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


            if (GUILayout.Button(new GUIContent("Select", "Hold Ctrl to select additional")))
            {
                entitySelectInteraction.Select();
            }
        }
    }

#endif
}