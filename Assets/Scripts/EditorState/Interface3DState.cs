using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class Interface3DState
    {
        private Subscribable<GameObject> _currentlyHoveredObject = new();
        public Subscribable<GameObject> CurrentlyHoveredObject => _currentlyHoveredObject;

        public void SetCurrentlyHoveredObject(GameObject go)
        {
            if(_currentlyHoveredObject.Value != go)
                _currentlyHoveredObject.Value = go;
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
