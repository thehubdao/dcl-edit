using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.ProjectState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;

namespace Assets.Scripts.SceneVisuals
{
    public class GltfShapeVisuals : MonoBehaviour
    {
        private GameObject _currentModelObject = null;
        

        public void UpdateVisuals(DclEntity entity)
        {
            var assetGuid = entity.GetComponentByName("GLTFShape")?.GetPropertyByName("asset")?.GetConcrete<Guid>().Value;

            if (!assetGuid.HasValue)
                return;

            if (EditorStates.CurrentProjectState.Assets.UsedAssets.TryGetValue(assetGuid.Value, out var asset))
            {
                var gltfAsset = asset as DclGltfAsset;

                if (gltfAsset == null)
                    return;

                ModelCacheSystem.GetModel(gltfAsset.Path, "",
                    o =>
                    {
                        if (o == null)
                            return;

                        Destroy(_currentModelObject);

                        o.transform.SetParent(transform);
                        o.transform.localScale = Vector3.one;
                        o.transform.localRotation = Quaternion.identity;
                        o.transform.localPosition = Vector3.zero;

                        _currentModelObject = o;

                        UpdateSelection(entity);
                    });


            }

            UpdateSelection(entity);

        }

        private void UpdateSelection(DclEntity entity)
        {
            if (_currentModelObject == null)
                return;

            var selectionState = EditorStates.CurrentSceneState?.CurrentScene?.SelectionState;
            if (selectionState != null)
            {
                if (selectionState.PrimarySelectedEntity == entity)
                {
                    SetLayer(_currentModelObject.gameObject, LayerMask.NameToLayer("Outline2"));
                }
                else if (selectionState.SecondarySelectedEntities.Contains(entity))
                {
                    SetLayer(_currentModelObject.gameObject, LayerMask.NameToLayer("Outline3"));
                }
                else
                {
                    SetLayer(_currentModelObject.gameObject, LayerMask.NameToLayer("Default"));
                }
            }
        }

        private void SetLayer(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetLayer(child.gameObject, layer);
            }
        }

        public void Deactivate()
        {
            _currentModelObject.SetActive(false);
        }
    }
}
