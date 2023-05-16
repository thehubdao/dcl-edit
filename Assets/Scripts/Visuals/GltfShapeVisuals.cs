using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GltfShapeVisuals : ShapeVisuals
    {
        private GameObject _currentModelObject = null;

        // Dependencies
        private AssetManagerSystem _assetManagerSystem;
        private UnityState _unityState;
        private SceneManagerSystem _sceneManagerSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            UnityState unityState,
            SceneManagerSystem sceneManagerSystem)
        {
            _assetManagerSystem = assetManagerSystem;
            _unityState = unityState;
            _sceneManagerSystem = sceneManagerSystem;
        }

        public override void UpdateVisuals(DclScene scene, DclEntity entity)
        {
            var assetGuid = entity.GetComponentByName("GLTFShape")?.GetPropertyByName("asset")?.GetConcrete<Guid>().Value;

            if (!assetGuid.HasValue)
                return;

            var data = _assetManagerSystem.GetDataById((Guid)assetGuid);

            GameObject newModel = null;
            switch (data.state)
            {
                case AssetData.State.IsAvailable:
                    if (data is ModelAssetData)
                    {
                        ModelAssetData modelData = (ModelAssetData) data;
                        if (modelData.data == null)
                            return;

                        newModel = modelData.data;
                    }

                    break;

                case AssetData.State.IsLoading:
                    newModel = Instantiate(_unityState.LoadingModel);
                    break;

                case AssetData.State.IsError:
                    newModel = Instantiate(_unityState.ErrorModel);
                    break;
            }

            if (newModel != null)
            {
                Destroy(_currentModelObject);

                newModel.transform.SetParent(transform);
                newModel.transform.localScale = Vector3.one;
                newModel.transform.localRotation = Quaternion.identity;
                newModel.transform.localPosition = Vector3.zero;

                _currentModelObject = newModel;

                UpdateSelection(entity);
            }

            if (GetComponentInParent<FloatingVisualsMarker>() != null)
            {
                StaticUtilities.SetLayerRecursive(gameObject, LayerMask.NameToLayer("Ignore Raycast"));
            }
        }

        public override void Deactivate()
        {
            _currentModelObject.SetActive(false);
        }

        public class Factory : PlaceholderFactory<GltfShapeVisuals>
        {
        }
    }
}
