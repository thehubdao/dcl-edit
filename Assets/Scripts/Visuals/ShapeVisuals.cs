using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class ShapeVisuals : MonoBehaviour
    {
        protected const int EntityClickLayer = 10;

        // Dependencies
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(SceneManagerSystem sceneManagerSystem)
        {
            this.sceneManagerSystem = sceneManagerSystem;
        }


        public virtual void UpdateVisuals(DclScene scene, DclEntity entity)
        {
        }


        protected void UpdateSelection(DclEntity entity)
        {
            var selectionState = sceneManagerSystem.GetCurrentSceneOrNull()?.SelectionState;

            if (selectionState == null)
            {
                return;
            }

            if (selectionState.PrimarySelectedEntity.Value == entity)
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

        public void ShowPrimarySelectionOutline()
        {
            SetRenderingLayerRecursive(gameObject, LayerMask.NameToLayer("Outline2"));
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