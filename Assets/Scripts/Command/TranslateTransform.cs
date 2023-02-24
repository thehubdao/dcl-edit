using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

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

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.position.SetFixedValue(newFixedPosition);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.position.SetFixedValue(oldFixedPosition);
            editorEvents.InvokeSelectionChangedEvent();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityById(guid)?.GetTransformComponent() ?? null;
        }
    }
}
