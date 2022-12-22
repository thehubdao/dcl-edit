using Assets.Scripts.Events;
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
                _editorEvents.InvokeSelectionChangedEvent();
            }
        }

        // When holding a gizmo this struct contains information about current gizmo operation.
        public GizmoData? currentGizmoData;

        public struct GizmoData
        {
            // The direction in which the gizmo is dragged. In world space.
            public Vector3? dragAxis;

            // Dragging on a plane (e.g. xy-plane) or on a single axis?
            public bool movingOnPlane;

            // The angle that the plane has to one of the world axis.
            public Quaternion planeRotation;
            public Quaternion inversePlaneRotation => Quaternion.Inverse(planeRotation);

            // The plane on which the mouse position is determined via raycasts.
            public Plane plane;

            // The initial mouse offset to the selected entity at the start of the gizmo drag.
            public Vector3 initialMouseOffset;

            // The axis around which to rotate. Only relevant when using a rotation gizmo. In local space.
            public Vector3? rotationAxis;

            public GizmoData(Plane mouseCollisionPlane, Vector3 initialMouseOffset, Quaternion planeRotation, bool movingOnPlane = true, Vector3? dragAxis = null, Vector3? rotationAxis = null)
            {
                this.dragAxis = dragAxis;
                this.movingOnPlane = movingOnPlane;
                this.planeRotation = planeRotation;
                this.plane = mouseCollisionPlane;
                this.initialMouseOffset = initialMouseOffset;
                this.rotationAxis = rotationAxis;
            }
        }

        public Vector2? snappingCarryOver;
    }
}