using System;
using System.Collections.Generic;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
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
        private CommandSystem commandSystem;
        private SelectionSystem selectionSystem;

        [Inject]
        private void Construct(
            GizmoState gizmoState,
            SceneManagerState sceneManagerState,
            CameraState cameraState,
            EditorEvents editorEvents,
            GizmoSizeSystem gizmoSizeSystem,
            SettingsSystem settingsSystem,
            CommandSystem commandSystem,
            SelectionSystem selectionSystem)
        {
            this.gizmoState = gizmoState;
            this.sceneManagerState = sceneManagerState;
            this.cameraState = cameraState;
            this.editorEvents = editorEvents;
            this.gizmoSizeSystem = gizmoSizeSystem;
            this.settingsSystem = settingsSystem;
            this.commandSystem = commandSystem;
            this.selectionSystem = selectionSystem;
        }

        public ToolMode gizmoToolMode
        {
            get => (ToolMode) settingsSystem.selectedGizmoTool.Get();
            set
            {
                settingsSystem.selectedGizmoTool.Set((int) value);

                editorEvents.InvokeUpdateSceneViewButtons();
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        public bool isToolSnapping
        {
            get => settingsSystem.gizmoToolDoesSnap.Get() > 0;
            set
            {
                settingsSystem.gizmoToolDoesSnap.Set(value ? 1 : 0);

                editorEvents.InvokeUpdateSceneViewButtons();
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        public ToolContext gizmoToolContext
        {
            get => (ToolContext) settingsSystem.gizmoLocalGlobalContext.Get();
            set
            {
                settingsSystem.gizmoLocalGlobalContext.Set((int) value);

                editorEvents.InvokeUpdateSceneViewButtons();
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

            var selectedEntities = selectionSystem.AllSelectedEntitiesWithoutChildren;

            if (selectedEntities != null)
            {
                List<DclTransformComponent> transforms = new List<DclTransformComponent>();
                foreach (var entity in selectedEntities)
                {
                    transforms.Add(entity?.GetTransformComponent());
                }
                gizmoState.multiselecTransforms = transforms;
            }

            // set GizmoRotation
            gizmoState.gizmoRotation =
                gizmoToolContext == ToolContext.Global ?
                    Quaternion.identity :
                    gizmoState.affectedTransform!.globalRotation;

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

            // set snapping offset, when its needed for grid snapping
            SetupSnappingOffset();
        }

        private void SetupSnappingOffset()
        {
            if (gizmoToolMode == ToolMode.Translate && gizmoToolContext == ToolContext.Global)
            {
                var startingEntityPosition = gizmoState.affectedTransform.globalPosition;

                if (gizmoState.gizmoDirection.isOnlyX())
                {
                    gizmoState.snappingOffset = new Vector2(
                        FindGridSnapOffset(startingEntityPosition.x, settingsSystem.gizmoToolTranslateSnapping.Get()),
                        0);
                }
                else if (gizmoState.gizmoDirection.isOnlyY())
                {
                    gizmoState.snappingOffset = new Vector2(
                        FindGridSnapOffset(startingEntityPosition.y, settingsSystem.gizmoToolTranslateSnapping.Get()),
                        0);
                }
                else if (gizmoState.gizmoDirection.isOnlyZ())
                {
                    gizmoState.snappingOffset = new Vector2(
                        FindGridSnapOffset(startingEntityPosition.z, settingsSystem.gizmoToolTranslateSnapping.Get()),
                        0);
                }
                else if (gizmoState.gizmoDirection.isXandY())
                {
                    gizmoState.snappingOffset = new Vector2(
                        FindGridSnapOffset(startingEntityPosition.x, settingsSystem.gizmoToolTranslateSnapping.Get()),
                        FindGridSnapOffset(startingEntityPosition.y, settingsSystem.gizmoToolTranslateSnapping.Get()));
                }
                else if (gizmoState.gizmoDirection.isXandZ())
                {
                    gizmoState.snappingOffset = new Vector2(
                        FindGridSnapOffset(startingEntityPosition.x, settingsSystem.gizmoToolTranslateSnapping.Get()),
                        FindGridSnapOffset(startingEntityPosition.z, settingsSystem.gizmoToolTranslateSnapping.Get()));
                }
                else if (gizmoState.gizmoDirection.isYandZ())
                {
                    gizmoState.snappingOffset = new Vector2(
                        FindGridSnapOffset(startingEntityPosition.y, settingsSystem.gizmoToolTranslateSnapping.Get()),
                        FindGridSnapOffset(startingEntityPosition.z, settingsSystem.gizmoToolTranslateSnapping.Get()));
                }
                else // if (gizmoState.gizmoDirection.isXYZ())
                {
                    gizmoState.snappingOffset = Vector2.zero;
                }
            }
            else
            {
                gizmoState.snappingOffset = Vector2.zero;
            }
        }

        private float FindGridSnapOffset(float startingValue, float snappingStep)
        {
            Assert.IsTrue(snappingStep > 0);
            return -(startingValue % snappingStep);
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
            // if direction is single axis
            if (gizmoState.gizmoDirection.isOnlyX() || gizmoState.gizmoDirection.isOnlyY() || gizmoState.gizmoDirection.isOnlyZ())
            {
                SetMouseContextForSingleAxisBasedTool(toolContext);
            }

            // if direction is two axis
            if (gizmoState.gizmoDirection.isXandZ() || gizmoState.gizmoDirection.isXandY() || gizmoState.gizmoDirection.isYandZ())
            {
                SetMouseContextForPlaneBasedTool(toolContext);
            }

            // if direction is all tree axis
            if (gizmoState.gizmoDirection.isXYZ())
            {
                SetMouseContextForAllAxisBasedTool(toolContext);
            }
        }

        private void SetMouseContextForSingleAxisBasedTool(ToolContext toolContext)
        {
            // extract center from transform context
            var centerPosition = gizmoState.affectedTransform.globalPosition;

            // extract rotation from transform context
            var contextRotation =
                toolContext == ToolContext.Local ?
                    gizmoState.affectedTransform.globalRotation :
                    Quaternion.identity;

            var primaryAxisBase =
                gizmoState.gizmoDirection.isX ?
                    new Vector3(1, 0, 0) :
                    gizmoState.gizmoDirection.isY ?
                        new Vector3(0, 1, 0) :
                        new Vector3(0, 0, 1);

            var primaryAxisRotated = contextRotation * primaryAxisBase;
            var secondaryAxisRotated = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxisRotated);

            gizmoState.SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated, mouseContextRelevance: OnlyPrimaryAxis);
        }

        private void SetMouseContextForPlaneBasedTool(ToolContext toolContext)
        {
            // extract center from transform context
            var centerPosition = gizmoState.affectedTransform.globalPosition;

            // extract rotation from transform context
            var contextRotation =
                toolContext == ToolContext.Local ?
                    gizmoState.affectedTransform.globalRotation :
                    Quaternion.identity;

            var primaryAxisBase = gizmoState.gizmoDirection.isX ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
            var secondaryAxisBase = gizmoState.gizmoDirection.isZ ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);

            var primaryAxisRotated = contextRotation * primaryAxisBase;
            var secondaryAxisRotated = contextRotation * secondaryAxisBase;

            gizmoState.SetMouseContext(centerPosition, primaryAxisRotated, secondaryAxisRotated, mouseContextRelevance: EntirePlane);
        }

        private void SetMouseContextForAllAxisBasedTool(ToolContext toolContext)
        {
            // extract center from transform context
            var centerPosition = gizmoState.affectedTransform.globalPosition;

            // extract rotation from transform context
            var contextRotation =
                toolContext == ToolContext.Local ?
                    gizmoState.affectedTransform.globalRotation :
                    Quaternion.identity;

            var primaryAxis = cameraState.Rotation * Vector3.right;
            var secondaryAxis = FigureOutMissingAxisBasedOnCameraPosition(centerPosition, primaryAxis);

            gizmoState.SetMouseContext(centerPosition, primaryAxis, secondaryAxis, mouseContextRelevance: EntirePlane);
        }


        private Vector3 FigureOutMissingAxisBasedOnCameraPosition(Vector3 centerPos, Vector3 primaryAxisVector)
        {
            var centerToCameraVector = VectorFromTo(centerPos, cameraState.Position);

            var perpendicularVector = Vector3.Cross(primaryAxisVector, centerToCameraVector);

            return perpendicularVector.normalized;
        }

        #endregion // Start holding

        #region While holding

        public void WhileHolding(Ray mouseRay, bool invertToolSnapping)
        {
            // calculate mouse on context plane
            var mousePos = RayOnPlane(mouseRay, gizmoState.mouseContextPlane);

            // get total mouse movement in mouse context space from start. Movement on Primary axis is in x, movement on Secondary axis is in y
            var contextSpaceMouseMovementSinceStart =
                new Vector2(
                    DistanceOnAxis(mousePos, true) - DistanceOnAxis(gizmoState.mouseStartingPosition, true),
                    DistanceOnAxis(mousePos, false) - DistanceOnAxis(gizmoState.mouseStartingPosition, false));

            if (isToolSnapping != invertToolSnapping)
            {
                var snappingDistance = gizmoToolMode switch
                {
                    ToolMode.Translate => settingsSystem.gizmoToolTranslateSnapping.Get(),
                    ToolMode.Rotate => settingsSystem.gizmoToolRotateSnapping.Get() / 90,
                    ToolMode.Scale => settingsSystem.gizmoToolScaleSnapping.Get(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                contextSpaceMouseMovementSinceStart.x = SnapValue(contextSpaceMouseMovementSinceStart.x, snappingDistance);
                contextSpaceMouseMovementSinceStart.y = SnapValue(contextSpaceMouseMovementSinceStart.y, snappingDistance);

                contextSpaceMouseMovementSinceStart += gizmoState.snappingOffset;
            }

            switch (gizmoToolMode)
            {
                case ToolMode.Translate:
                    TranslateWhileHolding(contextSpaceMouseMovementSinceStart);
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

        private void TranslateWhileHolding(Vector2 contextSpaceMouseMovementSinceStart)
        {
            var mouseMovementOnPrimaryAxis = gizmoState.mouseContextPrimaryVector * contextSpaceMouseMovementSinceStart.x;

            var mouseMovementOnSecondaryAxis =
                gizmoState.mouseContextRelevance == EntirePlane ?
                    gizmoState.mouseContextSecondaryVector * contextSpaceMouseMovementSinceStart.y :
                    Vector3.zero;

            var worldMouseMovementSinceStart = mouseMovementOnPrimaryAxis + mouseMovementOnSecondaryAxis;

            foreach (var transform in gizmoState.multiselecTransforms)
            {
                transform.globalPosition = transform.globalFixedPosition + worldMouseMovementSinceStart;                
            }
        }

        private void RotateWhileHolding(Vector2 contextSpaceMouseMovementSinceStart)
        {
            var additionalRotation = Quaternion.Euler(
                gizmoState.gizmoDirection.isOnlyX() ? contextSpaceMouseMovementSinceStart.x * -90 : 0,
                gizmoState.gizmoDirection.isOnlyY() ? contextSpaceMouseMovementSinceStart.x * -90 : 0,
                gizmoState.gizmoDirection.isOnlyZ() ? contextSpaceMouseMovementSinceStart.x * -90 : 0);

            DclTransformComponent pivotTransform = gizmoState.affectedTransform;
            Vector3 pivotPosition = pivotTransform.globalFixedPosition;

            var additionalWorldRotation = gizmoState.gizmoRotation * additionalRotation * Quaternion.Inverse(gizmoState.gizmoRotation);

            if (gizmoToolContext == ToolContext.Local)
            {
                foreach (var transform in gizmoState.multiselecTransforms)
                {
                    transform.SetGlobalPivotRotation(pivotPosition, additionalWorldRotation);
                }
            }
            else
            {
                foreach (var transform in gizmoState.multiselecTransforms)
                {
                    transform.SetGlobalPivotRotation(pivotPosition, additionalWorldRotation);
                }
            }
        }

        private void ScaleWhileHolding(Vector2 contextSpaceMouseMovementSinceStart)
        {
            var additionalScale = new Vector3(
                gizmoState.gizmoDirection.isX ? contextSpaceMouseMovementSinceStart.x : 0,
                gizmoState.gizmoDirection.isY ? contextSpaceMouseMovementSinceStart.x : 0,
                gizmoState.gizmoDirection.isZ ? contextSpaceMouseMovementSinceStart.x : 0);

            foreach (var transform in gizmoState.multiselecTransforms)
            {
                transform.scale.SetFloatingValue(transform.scale.FixedValue + additionalScale);
            }
        }

        #endregion // While holding

        #region End holding

        public void EndHolding()
        {
            switch (gizmoToolMode)
            {
                case ToolMode.Translate:
                    ExecuteTranslateCommand();
                    break;
                case ToolMode.Rotate:
                    ExecuteRotateCommand();
                    break;
                case ToolMode.Scale:
                    ExecuteScaleCommand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CancelHolding()
        {
            switch (gizmoToolMode)
            {
                case ToolMode.Translate:
                    gizmoState.affectedTransform.globalPosition = gizmoState.affectedTransform.globalFixedPosition;
                    break;
                case ToolMode.Rotate:
                    gizmoState.affectedTransform.globalRotation = gizmoState.affectedTransform.globalFixedRotation;
                    break;
                case ToolMode.Scale:
                    gizmoState.affectedTransform.scale.SetFloatingValue(gizmoState.affectedTransform.scale.FixedValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            editorEvents.InvokeSelectionChangedEvent();
        }

        private void ExecuteTranslateCommand()
        {
            Assert.IsNotNull(gizmoState.multiselecTransforms);
            var list = new List<TranslateTransform.EntityTransform>();
            foreach (var transform in gizmoState.multiselecTransforms)
            {
                list.Add(new TranslateTransform.EntityTransform { 
                    selectedEntityGuid = transform.Entity.Id,
                    newFixedPosition = transform.position.Value,
                    oldFixedPosition = transform.position.FixedValue
                });
            }
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(list));
        }

        private void ExecuteRotateCommand()
        {
            Assert.IsNotNull(gizmoState.multiselecTransforms);
            var list = new List<RotateTransform.EntityTransform>();
            foreach (var transform in gizmoState.multiselecTransforms)
            {
                list.Add(new RotateTransform.EntityTransform
                {
                    selectedEntityGuid = transform.Entity.Id,
                    newFixedRotation = transform.rotation.Value,
                    oldFixedRotation = transform.rotation.FixedValue,
                    newFixedPosition = transform.position.Value,
                    oldFixedPosition = transform.position.FixedValue
                });
            }
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateRotateTransform(list));
        }

        private void ExecuteScaleCommand()
        {
            Assert.IsNotNull(gizmoState.multiselecTransforms);
            var list = new List<ScaleTransform.EntityTransform>();
            foreach (var transform in gizmoState.multiselecTransforms)
            {
                list.Add(new ScaleTransform.EntityTransform
                {
                    selectedEntityGuid = transform.Entity.Id,
                    newFixedScale = transform.scale.Value,
                    oldFixedScale = transform.scale.FixedValue
                });
            }
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateScaleTransform(list));
        }

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

        private float SnapValue(float value, float distance)
        {
            Assert.IsTrue(distance > 0);

            var normalizedValue = value / distance;
            var flooredValue = Mathf.Round(normalizedValue);
            var flooredValueInOriginalSpace = flooredValue * distance;
            return flooredValueInOriginalSpace;
        }

        #endregion
    }
}