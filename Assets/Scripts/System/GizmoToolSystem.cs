using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Assets.Scripts.System
{
    public class GizmoToolSystem
    {
        private class GizmoDirection
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

            public bool isXYZ()
            {
                return isX && isY && isZ;
            }


            public Vector3 GetDirectionVector()
            {
                return new Vector3(isX ? 1 : 0, isY ? 1 : 0, isZ ? 1 : 0);
            }
        }

        // Dependencies
        private GizmoState gizmoState;
        private SceneManagerState sceneManagerState;
        private CameraState cameraState;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(
            GizmoState gizmoState,
            SceneManagerState sceneManagerState,
            CameraState cameraState,
            EditorEvents editorEvents)
        {
            this.gizmoState = gizmoState;
            this.sceneManagerState = sceneManagerState;
            this.cameraState = cameraState;
            this.editorEvents = editorEvents;
        }

        public void SetGizmoMode(GizmoState.Mode value)
        {
            gizmoState.CurrentMode = value;
        }

        public void StartHolding(Vector3Int gizmoDirectionVector, Ray mouseRay)
        {
            // generate GizmoDirection
            var gizmoDirection = new GizmoDirection(gizmoDirectionVector);

            // get the transform context
            gizmoState.movedTransform = sceneManagerState.GetCurrentDirectoryState()?.currentScene?.SelectionState.PrimarySelectedEntity?.GetTransformComponent();
            Assert.IsNotNull(gizmoState.movedTransform);

            // set plane
            switch (gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    SetMouseContextForTransform(gizmoDirection, gizmoState.movedTransform);
                    break;
                case GizmoState.Mode.Rotate:
                    //SetPlaneForRotationByGizmoDirection(gizmoDirection);
                    break;
                case GizmoState.Mode.Scale:
                    //SetPlaneForScaleByGizmoDirection(gizmoDirection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // set starting mouse pos
            gizmoState.mouseStartingPosition = RayOnPlane(mouseRay, gizmoState.mouseContextPlane);
        }

        public void WhileHolding(Ray mouseRay)
        {
            // calculate mouse on context plane
            var mousePos = RayOnPlane(mouseRay, gizmoState.mouseContextPlane);

            // get total mouse movement from start
            var mouseMovementSinceStart = gizmoState.mouseStartingPosition.VectorTo(mousePos);


            switch (gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    TranslateWhileHolding(gizmoState.movedTransform, mouseMovementSinceStart);
                    break;
                case GizmoState.Mode.Rotate:
                    break;
                case GizmoState.Mode.Scale:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            editorEvents.InvokeSelectionChangedEvent();
        }

        private Vector3 RayOnPlane(Ray ray, Plane plane)
        {
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        private void TranslateWhileHolding(DclTransformComponent movedTransform, Vector3 movedVector)
        {
            movedTransform.GlobalPosition = movedTransform.GlobalFixedPosition + movedVector;
        }


        private void SetMouseContextForTransform(GizmoDirection gizmoDirection, DclTransformComponent transformContext)
        {
            // extract center from transform context
            var centerPosition = transformContext.GlobalPosition;

            // extract rotation from transform context
            var contextRotation = transformContext.GlobalRotation; // TODO: allow local/global context rotation

            // if direction is single axis
            if (gizmoDirection.isOnlyX() || gizmoDirection.isOnlyY() || gizmoDirection.isOnlyZ())
            {
                var primaryAxisBase =
                    gizmoDirection.isX ?
                        new Vector3(1, 0, 0) :
                        gizmoDirection.isY ?
                            new Vector3(0, 1, 0) :
                            new Vector3(0, 0, 1);

                var primaryAxisRotated = contextRotation * primaryAxisBase;
                var secondaryAxisRotated = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxisRotated);

                SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated);

                return;
            }

            // if direction is two axis
            if (gizmoDirection.isXandZ() || gizmoDirection.isXandY() || gizmoDirection.isYandZ())
            {
                var primaryAxisBase = gizmoDirection.isX ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
                var secondaryAxisBase = gizmoDirection.isZ ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);

                var primaryAxisRotated = contextRotation * primaryAxisBase;
                var secondaryAxisRotated = contextRotation * secondaryAxisBase;

                SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated);

                return;
            }

            // if direction is all tree axis
            if (gizmoDirection.isXYZ())
            {
                var primaryAxis = cameraState.Rotation * Vector3.right;
                var secondaryAxis = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxis);

                SetMouseContext(centerPosition, primaryAxis, secondaryAxis);

                return;
            }
        }

        private void SetMouseContext(Vector3 centerPos, Vector3 xVector, Vector3 yVector)
        {
            gizmoState.mouseContextCenter = centerPos;
            gizmoState.mouseContextXVector = xVector;
            gizmoState.mouseContextYVector = yVector;
        }

        private Vector3 FigureOutMissingAxisBasedOnCameraPosition(Vector3 centerPos, Vector3 primaryAxisVector)
        {
            var centerToCameraVector = centerPos.VectorTo(cameraState.Position);

            var perpendicularVector = Vector3.Cross(primaryAxisVector, centerToCameraVector);

            return perpendicularVector.normalized;
        }
    }
}
