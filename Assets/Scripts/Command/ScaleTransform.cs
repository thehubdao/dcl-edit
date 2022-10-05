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

        public ScaleTransform(Guid selectedEntity, Vector3 oldFixedScale, Vector3 newFixedScale)
        {
            this.selectedEntityGuid = selectedEntity;
            this.oldFixedScale = oldFixedScale;
            this.newFixedScale = newFixedScale;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Scale.SetFixedValue(newFixedScale);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Scale.SetFixedValue(oldFixedScale);
            editorEvents.InvokeSelectionChangedEvent();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.AllEntities[guid]?.GetTransformComponent() ?? null;
        }
    }
}