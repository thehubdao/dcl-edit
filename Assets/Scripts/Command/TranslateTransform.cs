using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class TranslateTransform : SceneState.Command
    {
        public override string Name => "Move Transform";
        public override string Description => "Moving transform to new position.";
        public struct EntityTransform
        {
            public Guid selectedEntityGuid;
            public Vector3 oldFixedPosition;
            public Vector3 newFixedPosition;
        }

        private List<EntityTransform> entityTransforms;

        public TranslateTransform(List<EntityTransform> entityTransforms)
        {
            this.entityTransforms = entityTransforms;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            foreach (var entityTransform in entityTransforms)
            {
                DclTransformComponent transform = TransformFromEntityGuid(sceneState, entityTransform.selectedEntityGuid);
                transform?.position.SetFixedValue(entityTransform.newFixedPosition);
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            foreach(var entityTransform in entityTransforms)
            {
                DclTransformComponent transform = TransformFromEntityGuid(sceneState, entityTransform.selectedEntityGuid);
                transform?.position.SetFixedValue(entityTransform.oldFixedPosition);
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityById(guid)?.GetTransformComponent() ?? null;
        }
    }
}
