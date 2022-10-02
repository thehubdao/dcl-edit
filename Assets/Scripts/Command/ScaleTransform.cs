using UnityEngine;
using System;
using Assets.Scripts.SceneState;

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

        public override void Do(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Scale.SetFixedValue(newFixedScale);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        public override void Undo(DclScene sceneState)
        {
            DclTransformComponent transform = TransformFromEntityGuid(sceneState, selectedEntityGuid);
            transform?.Scale.SetFixedValue(oldFixedScale);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        DclTransformComponent TransformFromEntityGuid(DclScene sceneState, Guid guid)
        {
            return sceneState.GetEntityFormId(guid)?.GetTransformComponent() ?? null;
        }
    }
}