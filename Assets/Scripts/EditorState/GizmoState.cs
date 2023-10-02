using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        public class GizmoDirection
        {
            public readonly bool isX;
            public readonly bool isY;
            public readonly bool isZ;


            public GizmoDirection(Vector3Int gizmoDirection)
            {
                isX = gizmoDirection.x > 0;
                isY = gizmoDirection.y > 0;
                isZ = gizmoDirection.z > 0;
            }

            public bool isOnlyX()
            {
                return isX && !isY && !isZ;
            }

            public bool isOnlyY()
            {
                return !isX && isY && !isZ;
            }

            public bool isOnlyZ()
            {
                return !isX && !isY && isZ;
            }

            public bool isOneAxis()
            {
                return isOnlyX() || isOnlyY() || isOnlyZ();
            }

            public bool isXandY()
            {
                return isX && isY && !isZ;
            }

            public bool isXandZ()
            {
                return isX && !isY && isZ;
            }

            public bool isYandZ()
            {
                return !isX && isY && isZ;
            }

            public bool isTwoAxis()
            {
                return isXandY() || isXandZ() || isYandZ();
            }

            public bool isXYZ()
            {
                return isX && isY && isZ;
            }


            public Vector3 GetDirectionVector()
            {
                return new Vector3(isX ? 1 : 0, isY ? 1 : 0, isZ ? 1 : 0);
            }
        }

        public enum MouseContextRelevance
        {
            OnlyPrimaryAxis,
            EntirePlane
        }


        // While moving states

        public void SetMouseContext(Vector3 centerPos, Vector3 xVector, Vector3 yVector, GizmoState.MouseContextRelevance mouseContextRelevance)
        {
            mouseContextCenter = centerPos;
            mouseContextPrimaryVector = xVector;
            mouseContextSecondaryVector = yVector;
            this.mouseContextRelevance = mouseContextRelevance;
        }

        /// <summary>
        /// The transform, where the gizmo operations are applied to 
        /// </summary>
        public DclTransformComponent affectedTransform;

        /// <summary>
        /// IEnumrable of transforms, where gizmo operations are applied to when multiselect entities
        /// </summary>
        public IEnumerable<DclTransformComponent> multiselecTransforms;

        /// <summary>
        /// Describes the direction of the held gizmo tool
        /// </summary>
        public GizmoDirection gizmoDirection;

        /// <summary>
        /// Describes the center point of the mouse context
        /// </summary>
        /// <remarks>
        /// Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        /// </remarks>
        public Vector3 mouseContextCenter { get; private set; }

        /// <summary>
        /// Describes the X Vector. When the mouse points exactly at this vectors position, the resulting mouse movement coordinate will be (1,0). 
        /// </summary>
        /// <remarks>
        /// Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        /// </remarks>
        public Vector3 mouseContextPrimaryVector { get; private set; }

        /// <summary>
        /// Describes the Y Vector. When the mouse points exactly at this vectors position, the resulting mouse movement coordinate will be (0,1). 
        /// </summary>
        /// <remarks>
        /// Mouse position context. The center and the two Vectors describe the plane, on that the mouse position will be mapped in 3D space
        /// </remarks>
        public Vector3 mouseContextSecondaryVector { get; private set; }

        /// <summary>
        /// Tells, if only one axis or the entire plane is relevant
        /// </summary>
        public MouseContextRelevance mouseContextRelevance { get; private set; }

        /// <summary>
        /// The plane, that is defined by the position and the vectors
        /// </summary>
        public Plane mouseContextPlane => new Plane(mouseContextCenter, mouseContextCenter + mouseContextPrimaryVector, mouseContextCenter + mouseContextSecondaryVector);

        /// <summary>
        /// The starting position of the mouse on the plane
        /// </summary>
        public Vector3 mouseStartingPosition;

        /// <summary>
        /// The offset that is added to the position, when snapping is active. This is useful when the tool is in global translate mode, to have the snapping steps align with the world grid
        /// </summary>
        public Vector2 snappingOffset;
    }
}