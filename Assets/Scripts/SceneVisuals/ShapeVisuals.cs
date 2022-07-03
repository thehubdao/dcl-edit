using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

namespace Assets.Scripts.SceneVisuals
{
    public class ShapeVisuals : MonoBehaviour
    {
        protected const int EntityClickLayer = 10;

        public virtual void UpdateVisuals(DclEntity entity)
        {

        }

        

        protected void UpdateSelection(DclEntity entity)
        {

            var selectionState = EditorStates.CurrentSceneState?.CurrentScene?.SelectionState;
            if (selectionState != null)
            {
                if (selectionState.PrimarySelectedEntity == entity)
                {
                    SetRenderingLayerRecursive(gameObject, LayerMask.NameToLayer("Outline2"));
                }
                else if (selectionState.SecondarySelectedEntities.Contains(entity))
                {
                    SetRenderingLayerRecursive(gameObject, LayerMask.NameToLayer("Outline3"));
                }
                else
                {
                    SetRenderingLayerRecursive(gameObject, LayerMask.NameToLayer("Default"));
                }
            }
        }

        protected void SetRenderingLayerRecursive(GameObject o, int layer)
        {
            if (HasRenderer(o))
                o.layer = layer;

            foreach (Transform child in o.transform)
            {
                SetRenderingLayerRecursive(child.gameObject, layer);
            }
        }

        protected bool HasRenderer(GameObject o)
        {
            return o.TryGetComponent(out MeshRenderer _);
        }

        public virtual void Deactivate()
        {

        }
    }
}
