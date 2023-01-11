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
        public Guid id;

        // dependencies
        private EntitySelectSystem entitySelectSystem;

        [Inject]
        public void Construct(EntitySelectSystem entitySelectSystem)
        {
            this.entitySelectSystem = entitySelectSystem;
        }

        public void Select()
        {
            entitySelectSystem.ClickedOnEntity(id);
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