using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        // Dependencies
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(EditorEvents editorEvents)
        {
            _editorEvents = editorEvents;
        }

        public enum Mode
        {
            Translate,
            Rotate,
            Scale
        }

        private Mode _currentMode;

        public Mode CurrentMode
        {
            get => _currentMode;
            set
            {
                _currentMode = value;
                _editorEvents.InvokeGizmoModeChangeEvent();
                _editorEvents.InvokeSelectionChangedEvent();
            }
        }

        // While moving states

        // The transform, where the gizmo operations are applied to 
        public DclTransformComponent movedTransform;

        // Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        // Describes the center point of the mouse context
        public Vector3 mouseContextCenter;

        // Describes the X Vector. When the mouse points exactly at this vectors position, the resulting mouse movement coordinate will be (1,0). 
        public Vector3 mouseContextXVector;

        // Describes the Y Vector. When the mouse points exactly at this vectors position, the resulting mouse movement coordinate will be (0,1). 
        public Vector3 mouseContextYVector;

        // The plane, that is defined by the position and the vectors
        public Plane mouseContextPlane => new Plane(mouseContextCenter, mouseContextCenter + mouseContextXVector, mouseContextCenter + mouseContextYVector);

        // The starting position of the mouse on the plane
        public Vector3 mouseStartingPosition;
    }
}