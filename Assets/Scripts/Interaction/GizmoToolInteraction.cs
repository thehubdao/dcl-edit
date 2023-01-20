using System.Runtime.CompilerServices;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GizmoToolInteraction
    {
        // Dependencies
        private GizmoState gizmoState;
        private SceneManagerSystem sceneManagerSystem;
        private InputHelper inputHelper;
        private InputState inputState;
        private CommandSystem commandSystem;
        private UnityState unityState;
        private EditorEvents editorEvents;
        private Interface3DState interface3DState;
        private CameraState cameraState;
        private SettingsSystem settingsSystem;

        [Inject]
        private void Construct(
            GizmoState gizmoState,
            SceneManagerSystem sceneManagerSystem,
            InputHelper inputHelper,
            InputState inputState,
            CommandSystem commandSystem,
            UnityState unityState,
            EditorEvents editorEvents,
            Interface3DState interface3DState,
            CameraState cameraState,
            SettingsSystem settingsSystem)
        {
            this.gizmoState = gizmoState;
            this.sceneManagerSystem = sceneManagerSystem;
            this.inputHelper = inputHelper;
            this.inputState = inputState;
            this.commandSystem = commandSystem;
            this.unityState = unityState;
            this.editorEvents = editorEvents;
            this.interface3DState = interface3DState;
            this.cameraState = cameraState;
            this.settingsSystem = settingsSystem;
        }

        public void ClickOnGizmoTool(Vector3 mousePositionIn3DView)
        {
            inputState.InState = InputState.InStateType.HoldingGizmoTool;

            GizmoDirection gizmoDir = interface3DState.CurrentlyHoveredObject.GetComponent<GizmoDirection>();
            if (gizmoDir == null) return;


            Vector3 localGizmoDir = gizmoDir.GetVector();

            var entity = sceneManagerSystem.GetCurrentScene()?.SelectionState.PrimarySelectedEntity.GetTransformComponent();

            GizmoState.Mode gizmoMode = gizmoState.CurrentMode;

            // Two special cases when in translation mode:
            // Dragging on planes is handled differently from the rest of the gizmo operations.
            if (gizmoMode == GizmoState.Mode.Translate)
            {
                // Dragging the center block of the translation gizmo (all three axis active in GizmoDirection component)
                if (localGizmoDir == Vector3.one)
                {
                    Vector3 planeNormal = cameraState.Position - entity.GlobalPosition;
                    Plane p = new Plane(planeNormal, entity.GlobalPosition);

                    // Calculate initial mouse offset to selected object
                    Ray r = unityState.MainCamera.ViewportPointToRay(inputHelper.GetMousePositionInScenePanel());
                    if (p.Raycast(r, out float enterDistance))
                    {
                        Vector3 hitPoint = r.GetPoint(enterDistance);
                        Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;
                        var planeRotation = Quaternion.FromToRotation(planeNormal, new Vector3(1, 0, 0));

                        gizmoState.currentGizmoData = new GizmoState.GizmoData(p, dirToHitPoint, planeRotation);
                    }

                    return;
                }
                //else

                // Dragging on a plane (XY plane, XZ plane or YZ plane)
                if ((gizmoDir.x && gizmoDir.y || gizmoDir.x && gizmoDir.z || gizmoDir.y && gizmoDir.z))
                {
                    // Get the normal by inverting the direction vector. That way the one direction that is 0 becomes 1 which is the normal direction.
                    Vector3 planeNormal = Vector3.one - localGizmoDir;

                    // Transform normal into world space
                    planeNormal = entity.TransformPoint(planeNormal) - entity.GlobalPosition;

                    Plane p = new Plane(planeNormal, entity.GlobalPosition);

                    // Calculate initial mouse offset to selected object
                    Ray r = unityState.MainCamera.ViewportPointToRay(inputHelper.GetMousePositionInScenePanel());
                    if (p.Raycast(r, out float enterDistance))
                    {
                        Vector3 hitPoint = r.GetPoint(enterDistance);
                        Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;
                        var planeRotation = Quaternion.FromToRotation(planeNormal, new Vector3(1, 0, 0));

                        gizmoState.currentGizmoData = new GizmoState.GizmoData(p, dirToHitPoint, planeRotation);
                    }

                    return;
                }
            }


            // Along this axis the displacement of the mouse position is measured. It is also used to create a plane which handles
            // the collision detection for the mouse position in 3d space while using gizmos.
            Vector3 gizmoAxis;

            if (gizmoMode == GizmoState.Mode.Rotate)
            {
                Vector3 localMousePos = entity.InverseTransformPoint((Vector3) mousePositionIn3DView);

                // Clean up the local space mouse position
                // Example: if we rotate the object around the x-axis we want a local space mouse position where
                // the x-value is 0. That is because the gizmos have colliders so the 3d mouse position will
                // probably be a bit offset from the actual gizmo position.
                Vector3 invertedGizmoDir = Vector3.one - localGizmoDir; // Remove the rotation axis by setting it to 0
                localMousePos = Vector3.Scale(localMousePos, invertedGizmoDir);

                // Calculate the gizmo axis
                // Gizmo axis must be perpendicular to both the direction to local mouse position and the gizmo direction.
                // That way the gizmo axis matches the slope of the rotation gizmo at the clicked position. 
                Vector3 localGizmoAxis = Vector3.Cross(localMousePos, localGizmoDir);

                // Transform gizmo axis to world space
                gizmoAxis = entity.TransformPoint(localMousePos + localGizmoAxis) - entity.TransformPoint(localMousePos);
            }
            else
            {
                gizmoAxis = (entity.TransformPoint(localGizmoDir) - entity.GlobalPosition).normalized;
            }

            // Then make sure that dirToCamera is orthogonal to gizmoAxis. This orthogonal dirToCamera vector
            // now becomes the normal of the plane which does the mouse collision.
            Vector3 dirToCamera = cameraState.Position - entity.GlobalPosition;
            Vector3.OrthoNormalize(ref gizmoAxis, ref dirToCamera);
            Vector3 normal = dirToCamera;

            // Create a plane on which the mouse position is determined via raycasts. Lies along gizmo axis.
            Plane plane;
            if (gizmoMode == GizmoState.Mode.Rotate)
            {
                plane = new Plane(normal, (Vector3) mousePositionIn3DView);
            }
            else
            {
                plane = new Plane(normal, entity.GlobalPosition);
            }

            // Calculate initial mouse offset to selected object
            Ray ray = unityState.MainCamera.ViewportPointToRay(inputHelper.GetMousePositionInScenePanel());

            if (!plane.Raycast(ray, out float enter))
            {
                return;
            }

            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;

                Quaternion gizmoRotation = Quaternion.FromToRotation(gizmoAxis, new Vector3(1, 0, 0));

                // Pass data about the current gizmo operation to the update method.
                gizmoState.currentGizmoData = new GizmoState.GizmoData(plane, dirToHitPoint, gizmoRotation, false, dragAxis: gizmoAxis.normalized, rotationAxis: localGizmoDir);
            }
        }


        public void UpdateHoldingGizmoTool()
        {
            GizmoState.Mode mode = gizmoState.CurrentMode;
            DclEntity selectedEntity = sceneManagerSystem.GetCurrentScene()?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                return;
            }

            DclTransformComponent trans = selectedEntity!.GetTransformComponent();

            // When releasing LMB, stop holding gizmo
            if (!inputHelper.IsLeftMouseButtonPressed())
            {
                inputState.InState = InputState.InStateType.NoInput;
                gizmoState.currentGizmoData = null;

                switch (mode)
                {
                    case GizmoState.Mode.Translate:
                        commandSystem.ExecuteCommand(
                            commandSystem.CommandFactory.CreateTranslateTransform(selectedEntity.Id, trans.Position.FixedValue, trans.Position.Value)
                        );
                        break;
                    case GizmoState.Mode.Rotate:
                        commandSystem.ExecuteCommand(
                            commandSystem.CommandFactory.CreateRotateTransform(selectedEntity.Id, trans.Rotation.FixedValue, trans.Rotation.Value)
                        );
                        break;
                    case GizmoState.Mode.Scale:
                        commandSystem.ExecuteCommand(
                            commandSystem.CommandFactory.CreateScaleTransform(selectedEntity.Id, trans.Scale.FixedValue, trans.Scale.Value)
                        );
                        break;
                }

                return;
            }

            if (gizmoState.currentGizmoData == null)
            {
                return;
            }

            GizmoState.GizmoData gizmoData = (GizmoState.GizmoData) gizmoState.currentGizmoData;

            // Find mouse position in world on previously calculated plane
            Ray ray = unityState.MainCamera.ViewportPointToRay(inputHelper.GetMousePositionInScenePanel());
            if (!gizmoData.plane.Raycast(ray, out float enter))
            {
                return;
            }

            // Ignore mouse positions that are too far away
            if (enter >= unityState.MainCamera.farClipPlane)
            {
                return;
            }

            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dirToHitPoint = hitPoint - trans.GlobalFixedPosition;

            // Handle movement on planes separately:
            if (gizmoData.movingOnPlane && gizmoState.CurrentMode == GizmoState.Mode.Translate)
            {
                Vector3 globalPosition = gizmoData.plane.ClosestPointOnPlane(hitPoint - gizmoData.initialMouseOffset);
                Vector3? localPosition = selectedEntity.Parent?.GetTransformComponent().InverseTransformPoint(globalPosition);
                trans.Position.SetFloatingValue(localPosition ?? globalPosition);
                editorEvents.InvokeSelectionChangedEvent();
                return;
            }

            // Vector from the initial mouse position on the plane, to the current mouse position on the plane
            var hitPointWithOffset = dirToHitPoint - gizmoData.initialMouseOffset;

            // Check if the tool should snap
            var shouldSnap = settingsSystem.gizmoToolSnapping.Get() ^ inputHelper.GetIsControlPressed(); // ^ is the xor operator

            // if snapping is enabled, snap the value to the specified step
            if (shouldSnap)
            {
                var normalRotatedHitPoint = gizmoData.planeRotation * hitPointWithOffset;

                var snappedNormalRotatedHitPoint = new Vector3(Snap(normalRotatedHitPoint.x, 1f), Snap(normalRotatedHitPoint.y, 1f), Snap(normalRotatedHitPoint.z, 1f));

                hitPointWithOffset = gizmoData.inversePlaneRotation * snappedNormalRotatedHitPoint;
            }

            // Project the snappedHitPointWithOffset onto gizmoAxis. This results in a "shadow" of the snappedHitPointWithOffset which lies
            // on the gizmoAxis. Also factor in the mouse offset from the start of the drag to keep the object at the
            // same position relative to the mouse cursor. This point is relative to the selected object.
            Vector3 hitPointOnAxis = Vector3.Project(hitPointWithOffset, (Vector3) gizmoData.dragAxis);


            switch (gizmoState.CurrentMode)
            {
                case GizmoState.Mode.Translate:


                    Vector3 globalPosition = trans.GlobalFixedPosition + hitPointOnAxis;
                    Vector3? localPosition = selectedEntity.Parent?.GetTransformComponent().InverseTransformPoint(globalPosition);
                    trans.Position.SetFloatingValue(localPosition ?? globalPosition);
                    break;
                case GizmoState.Mode.Rotate:
                    // The distance along the gizmo axis at which the hit point lies.
                    // If the hit point on axis lies in the positive direction, the dot product returns 1. If it lies
                    // in the negative direction, the dot product returns -1. Therefore we can determine how far we pointed
                    // along the gizmo axis and in which direction.
                    float signedHitDistance = Vector3.Dot(hitPointOnAxis.normalized, (Vector3) gizmoData.dragAxis) * hitPointOnAxis.magnitude;

                    // Measure the radius of the rotation gizmo circle. As the initial mouse position is pretty close
                    // to the circle we can take that as a radius.
                    float radius = gizmoData.initialMouseOffset.magnitude;

                    // The distance moved by the mouse is the length of the arc. 
                    float arcLength = signedHitDistance;

                    // Calculate the angle of the arc. This is the amount that the object will be rotated by.
                    float angle = (arcLength * 360) / (2 * Mathf.PI * radius);

                    // Invert to rotate in the correct direction
                    angle *= -1;

                    Quaternion newRotation = trans.Rotation.FixedValue * Quaternion.Euler((Vector3) gizmoData.rotationAxis * angle);

                    trans.Rotation.SetFloatingValue(newRotation);
                    break;
                case GizmoState.Mode.Scale:
                    Vector3 currentScale = trans.Scale.FixedValue;

                    // The point on the gizmo axis that is closest to the current mouse position. Transformed into local space.
                    Vector3 localHitPointOnAxis = Quaternion.Inverse(trans.GlobalRotation) * hitPointOnAxis;
                    Vector3 newScale = currentScale + Vector3.Scale(currentScale, localHitPointOnAxis);
                    trans.Scale.SetFloatingValue(newScale);
                    break;
            }

            editorEvents.InvokeSelectionChangedEvent();
        }

        private float Snap(float value, float step)
        {
            var normalized = value / step;
            var floored = Mathf.Floor(normalized);
            return floored * step;
        }
    }
}