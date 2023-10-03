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
        private SceneJsonReaderSystem _sceneJsonReaderSystem;

        [Inject]
        private void Construct(
            AssetManagerSystem assetManagerSystem,
            UnityState unityState,
            SceneManagerSystem sceneManagerSystem,
            SceneJsonReaderSystem sceneJsonReaderSystem)
        {
            _assetManagerSystem = assetManagerSystem;
            _unityState = unityState;
            _sceneManagerSystem = sceneManagerSystem;
            _sceneJsonReaderSystem = sceneJsonReaderSystem;
        }

        public override void UpdateVisuals(DclScene scene, DclEntity entity)
        {
            var assetGuid =
                _sceneJsonReaderSystem.IsEcs7() ?
                    entity
                        .GetComponentByName(DclGltfContainerComponent.gltfShapeComponentDefinition.NameInCode)?
                        .GetPropertyByName("src")?
                        .GetConcrete<Guid>()
                        .Value :
                    entity
                        .GetComponentByName("GLTFShape")?
                        .GetPropertyByName("asset")?
                        .GetConcrete<Guid>()
                        .Value;

            if (!assetGuid.HasValue)
                return;

            var data = _assetManagerSystem.GetDataById((Guid)assetGuid);

            GameObject newModel = null;
            switch (data.state)
            {
                case AssetData.State.IsAvailable:
                    if (data is ModelAssetData modelData)
                    {
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

                if (newModel.TryGetComponent(out Animation animation))
                    animation.enabled = false;

                UpdateSelection(entity);
            }

            if (scene.IsFloatingEntity(entity.Id)! == true)
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
