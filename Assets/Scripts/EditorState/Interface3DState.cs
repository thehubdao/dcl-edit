using Assets.Scripts.System;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class Interface3DState
    {
        private GameObject _currentlyHoveredObject = null;

        // Dependencies
        EditorEvents _editorEvents;

        public Interface3DState(EditorEvents editorEvents)
        {
            _editorEvents = editorEvents;
        }


        public GameObject CurrentlyHoveredObject
        {
            get => _currentlyHoveredObject;
            set
            {
                if (_currentlyHoveredObject != value)
                {
                    _currentlyHoveredObject = value;
                    _editorEvents.HoverChangedEvent();
                }
            }
        }

        public enum HoveredObjectType
        {
            None,
            Gizmo,
            Entity
        }

        public HoveredObjectType CurrentlyHoveredObjectType = HoveredObjectType.None;
    }
}
