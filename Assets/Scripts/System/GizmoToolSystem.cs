using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using static Assets.Scripts.EditorState.GizmoState.MouseContextRelevance;

namespace Assets.Scripts.System
{
    public class GizmoToolSystem
    {
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

        #region Start holding

        public void StartHolding(Vector3Int gizmoDirectionVector, Ray mouseRay)
        {
            // generate GizmoDirection
            gizmoState.gizmoDirection = new GizmoState.GizmoDirection(gizmoDirectionVector);

            // get the transform context
            gizmoState.affectedTransform = sceneManagerState.GetCurrentDirectoryState()?.currentScene?.SelectionState.PrimarySelectedEntity?.GetTransformComponent();
            Assert.IsNotNull(gizmoState.affectedTransform);

            // set plane
            switch (gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    SetMouseContextForTranslate();
                    break;
                case GizmoState.Mode.Rotate:
                    SetMouseContextForRotate();
                    break;
                case GizmoState.Mode.Scale:
                    SetMouseContextForScale();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // set starting mouse pos
            gizmoState.mouseStartingPosition = RayOnPlane(mouseRay, gizmoState.mouseContextPlane);
        }


        private void SetMouseContextForTranslate()
        {
            SetMouseContextForAxisBasedTools();
        }

        private void SetMouseContextForRotate()
        {
            throw new NotImplementedException();
        }

        private void SetMouseContextForScale()
        {
            Assert.IsFalse(gizmoState.gizmoDirection.isXandZ() || gizmoState.gizmoDirection.isXandY() || gizmoState.gizmoDirection.isYandZ(), "Scale gizmo tool can only have one or all tree axis set");

            SetMouseContextForAxisBasedTools();
        }

        private void SetMouseContextForAxisBasedTools()
        {
            // extract center from transform context
            var centerPosition = gizmoState.affectedTransform.globalPosition;

            // extract rotation from transform context
            var contextRotation = gizmoState.affectedTransform.globalRotation; // TODO: allow local/global context rotation. Only local context, currently

            // if direction is single axis
            if (gizmoState.gizmoDirection.isOnlyX() || gizmoState.gizmoDirection.isOnlyY() || gizmoState.gizmoDirection.isOnlyZ())
            {
                var primaryAxisBase =
                    gizmoState.gizmoDirection.isX ?
                        new Vector3(1, 0, 0) :
                        gizmoState.gizmoDirection.isY ?
                            new Vector3(0, 1, 0) :
                            new Vector3(0, 0, 1);

                var primaryAxisRotated = contextRotation * primaryAxisBase;
                var secondaryAxisRotated = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxisRotated);

                SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated, mouseContextRelevance: OnlyPrimaryAxis);

                return;
            }

            // if direction is two axis
            if (gizmoState.gizmoDirection.isXandZ() || gizmoState.gizmoDirection.isXandY() || gizmoState.gizmoDirection.isYandZ())
            {
                var primaryAxisBase = gizmoState.gizmoDirection.isX ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
                var secondaryAxisBase = gizmoState.gizmoDirection.isZ ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);

                var primaryAxisRotated = contextRotation * primaryAxisBase;
                var secondaryAxisRotated = contextRotation * secondaryAxisBase;

                SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated, mouseContextRelevance: EntirePlane);

                return;
            }

            // if direction is all tree axis
            if (gizmoState.gizmoDirection.isXYZ())
            {
                var primaryAxis = cameraState.Rotation * Vector3.right;
                var secondaryAxis = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxis);

                SetMouseContext(centerPosition, primaryAxis, secondaryAxis, mouseContextRelevance: EntirePlane);

                return; // ReSharper disable once RedundantJumpStatement
            }
        }

        private void SetMouseContext(Vector3 centerPos, Vector3 xVector, Vector3 yVector, GizmoState.MouseContextRelevance mouseContextRelevance)
        {
            gizmoState.mouseContextCenter = centerPos;
            gizmoState.mouseContextPrimaryVector = xVector;
            gizmoState.mouseContextSecondaryVector = yVector;
            gizmoState.mouseContextRelevance = mouseContextRelevance;
        }

        private Vector3 FigureOutMissingAxisBasedOnCameraPosition(Vector3 centerPos, Vector3 primaryAxisVector)
        {
            var centerToCameraVector = centerPos.VectorTo(cameraState.Position);

            var perpendicularVector = Vector3.Cross(primaryAxisVector, centerToCameraVector);

            return perpendicularVector.normalized;
        }

        #endregion // Start holding

        #region While holding

        public void WhileHolding(Ray mouseRay)
        {
            // calculate mouse on context plane
            var mousePos = RayOnPlane(mouseRay, gizmoState.mouseContextPlane);

            // get total mouse movement in world space from start
            var worldSpaceMouseMovementSinceStart = gizmoState.mouseStartingPosition.VectorTo(mousePos);

            // if the currently hold tool uses only one axis and not a plane, project the moved vector onto the primary mouse context axis
            if (gizmoState.mouseContextRelevance == OnlyPrimaryAxis)
            {
                worldSpaceMouseMovementSinceStart = Vector3.Project(worldSpaceMouseMovementSinceStart, gizmoState.mouseContextPrimaryVector);
            }

            // get total mouse movement in mouse context space from start. Movement on Primary axis is in x, movement on Secondary axis is in y
            var contextSpaceMouseMovementSinceStart =
                new Vector2(
                    DistanceOnAxis(mousePos, true) - DistanceOnAxis(gizmoState.mouseStartingPosition, true),
                    DistanceOnAxis(mousePos, false) - DistanceOnAxis(gizmoState.mouseStartingPosition, false));

            switch (gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:
                    TranslateWhileHolding(worldSpaceMouseMovementSinceStart);
                    break;
                case GizmoState.Mode.Rotate:
                    throw new NotImplementedException();
                    break;
                case GizmoState.Mode.Scale:
                    ScaleWhileHolding(contextSpaceMouseMovementSinceStart);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            editorEvents.InvokeSelectionChangedEvent();
        }


        private void TranslateWhileHolding(Vector3 mouseMovementVectorSinceStart)
        {
            gizmoState.affectedTransform.globalPosition = gizmoState.affectedTransform.globalFixedPosition + mouseMovementVectorSinceStart;
        }

        private void ScaleWhileHolding(Vector2 contextSpaceMouseMovementSinceStart)
        {
            var additionalScale = new Vector3(
                gizmoState.gizmoDirection.isX ? contextSpaceMouseMovementSinceStart.x : 0,
                gizmoState.gizmoDirection.isY ? contextSpaceMouseMovementSinceStart.x : 0,
                gizmoState.gizmoDirection.isZ ? contextSpaceMouseMovementSinceStart.x : 0);

            gizmoState.affectedTransform.scale.SetFloatingValue(gizmoState.affectedTransform.scale.FixedValue + additionalScale);
        }

        #endregion // While holding

        #region End holding

        #endregion // End holding


        #region Utility

        private Vector3 RayOnPlane(Ray ray, Plane plane)
        {
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        private float DistanceOnAxis(Vector3 worldPosition, bool primaryAxis = true)
        {
            var axis = primaryAxis ?
                gizmoState.mouseContextPrimaryVector :
                gizmoState.mouseContextSecondaryVector;

            var positionProjectedOnAxis = Vector3.Project(worldPosition, axis);

            var isProjectedPositionInNegativeDirection = Vector3.Angle(axis, positionProjectedOnAxis) > 90;

            var normalizedDistanceOnAxis = isProjectedPositionInNegativeDirection ? -positionProjectedOnAxis.magnitude : positionProjectedOnAxis.magnitude;

            var distanceOnAxis = normalizedDistanceOnAxis / axis.magnitude;

            return distanceOnAxis;
        }

        #endregion
    }
}
