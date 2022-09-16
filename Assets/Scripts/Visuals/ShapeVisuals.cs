using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Visuals
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
            {
                o.layer = layer;
            }

            foreach (var child in o.transform.GetChildren())
            {
                // don't apply the outline to objects that are ShapeVisuals
                // They represent different entities and have to manage their outline them self
                if (!child.TryGetComponent(out ShapeVisuals _))
                {
                    SetRenderingLayerRecursive(child.gameObject, layer);
                }
            }
        }

        protected bool HasRenderer(GameObject o)
        {
            return o.TryGetComponent(out MeshRenderer _) ||
                   o.TryGetComponent(out SkinnedMeshRenderer _);
        }

        public virtual void Deactivate()
        {

        }
    }
}
