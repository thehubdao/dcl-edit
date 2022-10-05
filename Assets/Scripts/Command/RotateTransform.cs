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

        // Dependencies
        private EditorEvents _editorEvents;

        public RotateTransform(Guid selectedEntity, Quaternion oldFixedRotation, Quaternion newFixedRotation, EditorEvents editorEvents)
        {
            this.selectedEntityGuid = selectedEntity;
            this.oldFixedRotation = oldFixedRotation;
            this.newFixedRotation = newFixedRotation;
            _editorEvents = editorEvents;
        }

        public override void Do(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Rotation.SetFixedValue(newFixedRotation);
            _editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Rotation.SetFixedValue(oldFixedRotation);
            _editorEvents.InvokeSelectionChangedEvent();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.AllEntities[guid]?.GetTransformComponent() ?? null;
        }
    }
}