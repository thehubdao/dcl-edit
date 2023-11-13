using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class ScaleTransform : SceneState.Command
    {
        public override string Name => "Scale Transform";
        public override string Description => "Scaling transform.";
        public struct EntityTransform
        {
            public Guid selectedEntityGuid;
            public Vector3 oldFixedScale;
            public Vector3 newFixedScale;
        }
        private List<EntityTransform> entityTransforms;

        public ScaleTransform(List<EntityTransform> entityTransforms)
        {
            this.entityTransforms = entityTransforms;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            foreach (var entityTransform in entityTransforms)
            {
                DclTransformComponent transform = TransformFromEntityGuid(sceneState, entityTransform.selectedEntityGuid);
                transform?.scale.SetFixedValue(entityTransform.newFixedScale);
                editorEvents.InvokeSelectionChangedEvent();

            }
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            foreach (var entityTransform in entityTransforms)
            {
                DclTransformComponent transform = TransformFromEntityGuid(sceneState, entityTransform.selectedEntityGuid);
                transform?.scale.SetFixedValue(entityTransform.oldFixedScale);
                editorEvents.InvokeSelectionChangedEvent();
            }
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityById(guid)?.GetTransformComponent() ?? null;
        }
    }
}