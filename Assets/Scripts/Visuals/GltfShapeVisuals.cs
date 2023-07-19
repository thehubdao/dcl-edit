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
        int currentRequest = 0;

        // Dependencies
        private AssetManagerSystem _assetManagerSystem;
        private UnityState _unityState;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            UnityState unityState)
        {
            _assetManagerSystem = assetManagerSystem;
            _unityState = unityState;
        }

        public override async void UpdateVisuals(DclScene scene, DclEntity entity)
        {
            // Save the counter of the function call. If a newer call was started, the old one becomes invalid.
            currentRequest++;
            int updateRequest = currentRequest;

            var assetGuid = entity.GetComponentByName("GLTFShape")?.GetPropertyByName("asset")?.GetConcrete<Guid>().Value;

            if (!assetGuid.HasValue)
                return;

            // Show loading icon
            UpdateModel(Instantiate(_unityState.LoadingModel));

            AssetData data = await _assetManagerSystem.GetDataById(assetGuid.Value);

            if (data == null) return;

            if (data is ModelAssetData mad)
            {
                // If this function call has expired, then delete the requested model and abort.
                if (updateRequest != currentRequest)
                {
                    Destroy(mad.data);
                    return;
                }

                // Display the model
                if (mad.data == null) return;
                UpdateModel(mad.data);
            }

            UpdateSelection(entity);

            if (scene.IsFloatingEntity(entity.Id)! == true)
            {
                StaticUtilities.SetLayerRecursive(gameObject, LayerMask.NameToLayer("Ignore Raycast"));
            }
        }

        void UpdateModel(GameObject newModel)
        {
            Destroy(_currentModelObject);

            newModel.transform.SetParent(transform);
            newModel.transform.localScale = Vector3.one;
            newModel.transform.localRotation = Quaternion.identity;
            newModel.transform.localPosition = Vector3.zero;

            _currentModelObject = newModel;
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
