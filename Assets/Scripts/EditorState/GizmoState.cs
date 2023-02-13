using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        public enum MouseContextRelevance
        {
            OnlyOneAxis,
            EntirePlane
        }

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

        /// <summary>
        /// The transform, where the gizmo operations are applied to 
        /// </summary>
        public DclTransformComponent movedTransform;

        /// <summary>
        /// Describes the center point of the mouse context
        /// </summary>
        /// <remarks>
        /// Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        /// </remarks>
        public Vector3 mouseContextCenter;

        /// <summary>
        /// Describes the X Vector. When the mouse points exactly at this vectors position, the resulting mouse movement coordinate will be (1,0). 
        /// </summary>
        /// <remarks>
        /// Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        /// </remarks>
        public Vector3 mouseContextPrimaryVector;

        /// <summary>
        /// Describes the Y Vector. When the mouse points exactly at this vectors position, the resulting mouse movement coordinate will be (0,1). 
        /// </summary>
        /// <remarks>
        /// Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        /// </remarks>
        public Vector3 mouseContextSecondaryVector;

        /// <summary>
        /// Tells, if only one axis or the entire plane is relevant
        /// </summary>
        public MouseContextRelevance mouseContextRelevance;

        /// <summary>
        /// The plane, that is defined by the position and the vectors
        /// </summary>
        public Plane mouseContextPlane => new Plane(mouseContextCenter, mouseContextCenter + mouseContextPrimaryVector, mouseContextCenter + mouseContextSecondaryVector);

        /// <summary>
        /// The starting position of the mouse on the plane
        /// </summary>
        public Vector3 mouseStartingPosition;
    }
}