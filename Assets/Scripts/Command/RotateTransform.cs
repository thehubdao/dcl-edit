using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class RotateTransform : SceneState.Command
    {
        public override string Name => "Rotate Transform";
        public override string Description => "Rotating transform.";
        Guid selectedEntityGuid;
        Quaternion oldFixedRotation;
        Quaternion newFixedRotation;

        public RotateTransform(Guid selectedEntity, Quaternion oldFixedRotation, Quaternion newFixedRotation)
        {
            this.selectedEntityGuid = selectedEntity;
            this.oldFixedRotation = oldFixedRotation;
            this.newFixedRotation = newFixedRotation;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Rotation.SetFixedValue(newFixedRotation);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Rotation.SetFixedValue(oldFixedRotation);
            editorEvents.InvokeSelectionChangedEvent();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityById(guid)?.GetTransformComponent() ?? null;
        }
    }
}