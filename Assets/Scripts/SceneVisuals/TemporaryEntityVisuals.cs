using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.SceneVisuals
{
    public class TemporaryEntityVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        public void SetupSceneEventListeners()
        {
            // when there is a scene loaded, add the visuals updater
            EditorStates.CurrentSceneState.CurrentScene?
                .HierarchyChangedEvent.AddListener(UpdateVisuals);

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (EditorStates.CurrentSceneState.CurrentScene == null)
                return;

            foreach (var entity in EditorStates.CurrentSceneState.CurrentScene.AllEntities.Select(e => e.Value))
            {
                var entityPos = entity.GetComponentByName("transform")?.GetPropertyByName("position")
                    ?.GetConcrete<Vector3>().Value;
                
                if (entityPos != null)
                    Debug.DrawRay(entityPos.Value, Vector3.up, Color.red, 100);
            }
        }
    }
}