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

        // Dependencies
        EditorEvents _editorEvents;

        public TranslateTransform(Guid selectedEntity, Vector3 oldFixedPosition, Vector3 newFixedPosition, EditorEvents editorEvents)
        {
            this.selectedEntityGuid = selectedEntity;
            this.oldFixedPosition = oldFixedPosition;
            this.newFixedPosition = newFixedPosition;
            _editorEvents = editorEvents;
        }

        public override void Do(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Position.SetFixedValue(newFixedPosition);
            _editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Position.SetFixedValue(oldFixedPosition);
            _editorEvents.InvokeSelectionChangedEvent();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.AllEntities[guid]?.GetTransformComponent() ?? null;
        }
    }
}
