using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class ScaleTransform : SceneState.Command
    {
        public override string Name => "Scale Transform";
        public override string Description => "Scaling transform.";
        Guid selectedEntityGuid;
        Vector3 oldFixedScale;
        Vector3 newFixedScale;

        // Dependencies
        private EditorEvents _editorEvents;

        public ScaleTransform(Guid selectedEntity, Vector3 oldFixedScale, Vector3 newFixedScale, EditorEvents editorEvents)
        {
            this.selectedEntityGuid = selectedEntity;
            this.oldFixedScale = oldFixedScale;
            this.newFixedScale = newFixedScale;
            _editorEvents = editorEvents;
        }

        public override void Do(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Scale.SetFixedValue(newFixedScale);
            _editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Scale.SetFixedValue(oldFixedScale);
            _editorEvents.InvokeSelectionChangedEvent();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.AllEntities[guid]?.GetTransformComponent() ?? null;
        }
    }
}