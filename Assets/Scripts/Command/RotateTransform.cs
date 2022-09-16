using UnityEngine;
using System;
using Assets.Scripts.SceneState;

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

        public override void Do(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Rotation.SetFixedValue(newFixedRotation);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        public override void Undo(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Rotation.SetFixedValue(oldFixedRotation);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.AllEntities[guid]?.GetTransformComponent() ?? null;
        }
    }
}