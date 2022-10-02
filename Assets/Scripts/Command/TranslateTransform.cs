using UnityEngine;
using System;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.Command
{
    public class TranslateTransform : SceneState.Command
    {
        public override string Name => "Move Transform";
        public override string Description => "Moving transform to new position.";
        Guid selectedEntityGuid;
        Vector3 oldFixedPosition;
        Vector3 newFixedPosition;

        public TranslateTransform(Guid selectedEntity, Vector3 oldFixedPosition, Vector3 newFixedPosition)
        {
            this.selectedEntityGuid = selectedEntity;
            this.oldFixedPosition = oldFixedPosition;
            this.newFixedPosition = newFixedPosition;
        }

        public override void Do(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Position.SetFixedValue(newFixedPosition);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        public override void Undo(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Position.SetFixedValue(oldFixedPosition);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityFormId(guid)?.GetTransformComponent() ?? null;
        }
    }
}
