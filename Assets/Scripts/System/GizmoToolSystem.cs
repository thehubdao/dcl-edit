using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Assertions;
using Zenject;
using static Assets.Scripts.EditorState.GizmoState.MouseContextRelevance;
using static Assets.Scripts.Utility.StaticUtilities;

namespace Assets.Scripts.System
{
    public class GizmoToolSystem
    {
        public enum ToolMode
        {
            Translate = 0,
            Rotate = 1,
            Scale = 2
        }

        public enum ToolContext
        {
            Local = 0,
            Global = 1
        }

        // Dependencies
        private GizmoState gizmoState;
        private SceneManagerState sceneManagerState;
        private CameraState cameraState;
        private EditorEvents editorEvents;
        private GizmoSizeSystem gizmoSizeSystem;
        private SettingsSystem settingsSystem;

        [Inject]
        private void Construct(
            GizmoState gizmoState,
            SceneManagerState sceneManagerState,
            CameraState cameraState,
            EditorEvents editorEvents,
            GizmoSizeSystem gizmoSizeSystem,
            SettingsSystem settingsSystem)
        {
            this.gizmoState = gizmoState;
            this.sceneManagerState = sceneManagerState;
            this.cameraState = cameraState;
            this.editorEvents = editorEvents;
            this.gizmoSizeSystem = gizmoSizeSystem;
            this.settingsSystem = settingsSystem;
        }

        public ToolMode gizmoToolMode
        {
            get => (ToolMode) settingsSystem.selectedGizmoTool.Get();
            set
            {
                settingsSystem.selectedGizmoTool.Set((int) value);

                editorEvents.InvokeGizmoModeChangeEvent();
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        public bool isToolSnapping
        {
            get => settingsSystem.gizmoToolDoesSnap.Get() > 0;
            set
            {
                settingsSystem.gizmoToolDoesSnap.Set(value ? 1 : 0);

                editorEvents.InvokeGizmoModeChangeEvent();
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        public ToolContext gizmoToolContext
        {
            get => (ToolContext) settingsSystem.gizmoLocalGlobalContext.Get();
            set
            {
                settingsSystem.gizmoLocalGlobalContext.Set((int) value);

                editorEvents.InvokeGizmoModeChangeEvent();
                editorEvents.InvokeSelectionChangedEvent();
            }
        }


        #region Start holding

        public void StartHolding(Vector3Int gizmoDirectionVector, Ray mouseRay)
        {
            // generate GizmoDirection
            gizmoState.gizmoDirection = new GizmoState.GizmoDirection(gizmoDirectionVector);

            // get the affected transform
            gizmoState.affectedTransform = sceneManagerState.GetCurrentDirectoryState()?.currentScene?.SelectionState.PrimarySelectedEntity?.GetTransformComponent();
            Assert.IsNotNull(gizmoState.affectedTransform);

            // set plane
            switch (gizmoToolMode)
            {
                case ToolMode.Translate:
                    SetMouseContextForTranslate();
                    break;
                case ToolMode.Rotate:
                    SetMouseContextForRotate(mouseRay);
                    break;
                case ToolMode.Scale:
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
            SetMouseContextForAxisBasedTools(gizmoToolContext);
        }

        private void SetMouseContextForRotate(Ray mouseRay)
        {
            // make plane in the middle of the tool and aligned with the grabbed torus
            var centerPosition = gizmoState.affectedTransform.globalPosition;
            var contextRotation =
                gizmoToolContext == ToolContext.Local ?
                    gizmoState.affectedTransform.globalRotation :
                    Quaternion.identity;

            var planeNormal = contextRotation * gizmoState.gizmoDirection.GetDirectionVector();
            var toolPlane = new Plane(planeNormal, centerPosition);

            // find the current mouse position on that plane
            var mousePositionOnPlane = RayOnPlane(mouseRay, toolPlane);

            // make a "click" vector from the tool center to that clicked position
            var clickVector = VectorFromTo(centerPosition, mousePositionOnPlane);

            // change that click vector to be of the length of the torus radius
            var clickVectorOnTorus = clickVector.normalized * gizmoSizeSystem.GetGizmoSize(centerPosition);

            // to get the center of the mouse context we need to add the tool center to the click vector
            var mouseContextCenterPosition = centerPosition + clickVectorOnTorus;

            // find perpendicular vector of the click vector and the normal of the torus plane
            var perpendicularVector = Vector3.Cross(clickVector, planeNormal).normalized;

            // change the perpendicular vector to be of the length half pi times tool radius
            var perpendicularVectorProperLength = perpendicularVector * gizmoSizeSystem.GetGizmoSize(centerPosition) * Mathf.PI / 2;

            // this is the primary axis of the mouse context
            // when moving the mouse by one in the mouse context that translates to a rotation of 90 degrees
            var mouseContextPrimaryAxis = perpendicularVectorProperLength;

            // set the secondary axis of the mouse context
            var mouseContextSecondaryAxis = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, mouseContextPrimaryAxis);

            gizmoState.SetMouseContext(mouseContextCenterPosition, mouseContextPrimaryAxis, mouseContextSecondaryAxis, OnlyPrimaryAxis);
        }

        private void SetMouseContextForScale()
        {
            Assert.IsFalse(gizmoState.gizmoDirection.isXandZ() || gizmoState.gizmoDirection.isXandY() || gizmoState.gizmoDirection.isYandZ(), "Scale gizmo tool can only have one or all tree axis set");

            // Scale can only be local
            SetMouseContextForAxisBasedTools(ToolContext.Local);
        }

        private void SetMouseContextForAxisBasedTools(ToolContext toolContext)
        {
            // extract center from transform context
            var centerPosition = gizmoState.affectedTransform.globalPosition;

            // extract rotation from transform context
            var contextRotation =
                toolContext == ToolContext.Local ?
                    gizmoState.affectedTransform.globalRotation :
                    Quaternion.identity;

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

                gizmoState.SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated, mouseContextRelevance: OnlyPrimaryAxis);

                return;
            }

            // if direction is two axis
            if (gizmoState.gizmoDirection.isXandZ() || gizmoState.gizmoDirection.isXandY() || gizmoState.gizmoDirection.isYandZ())
            {
                var primaryAxisBase = gizmoState.gizmoDirection.isX ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
                var secondaryAxisBase = gizmoState.gizmoDirection.isZ ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);

                var primaryAxisRotated = contextRotation * primaryAxisBase;
                var secondaryAxisRotated = contextRotation * secondaryAxisBase;

                gizmoState.SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated, mouseContextRelevance: EntirePlane);

                return;
            }

            // if direction is all tree axis
            if (gizmoState.gizmoDirection.isXYZ())
            {
                var primaryAxis = cameraState.Rotation * Vector3.right;
                var secondaryAxis = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxis);

                gizmoState.SetMouseContext(centerPosition, primaryAxis, secondaryAxis, mouseContextRelevance: EntirePlane);

                return; // ReSharper disable once RedundantJumpStatement
            }
        }


        private Vector3 FigureOutMissingAxisBasedOnCameraPosition(Vector3 centerPos, Vector3 primaryAxisVector)
        {
            var centerToCameraVector = VectorFromTo(centerPos, cameraState.Position);

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
            var worldSpaceMouseMovementSinceStart = VectorFromTo(gizmoState.mouseStartingPosition, mousePos);

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

            switch (gizmoToolMode)
            {
                case ToolMode.Translate:
                    TranslateWhileHolding(worldSpaceMouseMovementSinceStart);
                    break;
                case ToolMode.Rotate:
                    RotateWhileHolding(contextSpaceMouseMovementSinceStart);
                    break;
                case ToolMode.Scale:
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

        private void RotateWhileHolding(Vector2 contextSpaceMouseMovementSinceStart)
        {
            var additionalRotation = Quaternion.Euler(
                gizmoState.gizmoDirection.isOnlyX() ? contextSpaceMouseMovementSinceStart.x * -90 : 0,
                gizmoState.gizmoDirection.isOnlyY() ? contextSpaceMouseMovementSinceStart.x * -90 : 0,
                gizmoState.gizmoDirection.isOnlyZ() ? contextSpaceMouseMovementSinceStart.x * -90 : 0);

            if (gizmoToolContext == ToolContext.Local)
            {
                gizmoState.affectedTransform.rotation.SetFloatingValue(gizmoState.affectedTransform.rotation.FixedValue * additionalRotation);
            }
            else
            {
                gizmoState.affectedTransform.globalRotation = additionalRotation * gizmoState.affectedTransform.globalFixedRotation;
            }
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
