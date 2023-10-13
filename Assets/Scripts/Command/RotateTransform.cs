using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class RotateTransform : SceneState.Command
    {
        public override string Name => "Rotate Transform";
        public override string Description => "Rotating transform.";
        public struct EntityTransform
        {
            public Guid selectedEntityGuid;
            public Quaternion oldFixedRotation;
            public Quaternion newFixedRotation;
        }

        private List<EntityTransform> entityTransforms;

        public RotateTransform(List<EntityTransform> entityTransforms)
        {
            this.entityTransforms = entityTransforms;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            foreach (var entityTransform in entityTransforms)
            {
                DclTransformComponent transform = TransformFromEntityGuid(sceneState, entityTransform.selectedEntityGuid);
                transform?.rotation.SetFixedValue(entityTransform.newFixedRotation);
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            foreach(var entityTransform in entityTransforms)
            {
                DclTransformComponent transform = TransformFromEntityGuid(sceneState, entityTransform.selectedEntityGuid);
                transform?.rotation.SetFixedValue(entityTransform.oldFixedRotation);
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityById(guid)?.GetTransformComponent() ?? null;
        }
    }
}