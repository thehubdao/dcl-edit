using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using System;
using Assets.Scripts.Assets;
using JetBrains.Annotations;
using ModestTree;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Assert = UnityEngine.Assertions.Assert;

namespace Assets.Scripts.Visuals
{
    public class GltfShapeVisuals : ShapeVisuals
    {
        // Dependencies
        //private AssetManagerSystem assetManagerSystem;
        private UnityState unityState;
        private SceneManagerSystem sceneManagerSystem;
        private SceneJsonReaderSystem sceneJsonReaderSystem;
        private DiscoveredAssets discoveredAssets;
        private SpecialAssets specialAssets;

        [Inject]
        private void Construct(
            //AssetManagerSystem assetManagerSystem,
            UnityState unityState,
            SceneManagerSystem sceneManagerSystem,
            SceneJsonReaderSystem sceneJsonReaderSystem,
            DiscoveredAssets discoveredAssets,
            SpecialAssets specialAssets)
        {
            //this.assetManagerSystem = assetManagerSystem;
            this.unityState = unityState;
            this.sceneManagerSystem = sceneManagerSystem;
            this.sceneJsonReaderSystem = sceneJsonReaderSystem;
            this.discoveredAssets = discoveredAssets;
            this.specialAssets = specialAssets;
        }

        [CanBeNull]
        private CommonAssetTypes.AssetInfo displayedAsset = null;

        [CanBeNull]
        private CommonAssetTypes.ModelInstance currentModelObject = null;

        public override void UpdateVisuals(DclScene scene, DclEntity entity)
        {
            var assetGuid =
                sceneJsonReaderSystem.IsEcs7() ?
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
            {
                UpdateDisplayedAsset(null);
                return;
            }

            // check if asset exists
            if (discoveredAssets.discoveredAssets.TryGetValue(assetGuid.Value, out var asset))
            {
                UpdateDisplayedAsset(asset);
            }
            else
            {
                UpdateDisplayedAsset(null);
            }


            //var data = _assetManagerSystem.GetDataById((Guid)assetGuid);
            //
            //GameObject newModel = null;
            //switch (data.state)
            //{
            //    case AssetData.State.IsAvailable:
            //        if (data is ModelAssetData modelData)
            //        {
            //            if (modelData.data == null)
            //                return;
            //
            //            newModel = modelData.data;
            //        }
            //
            //        break;
            //
            //    case AssetData.State.IsLoading:
            //        newModel = Instantiate(_unityState.LoadingModel);
            //        break;
            //
            //    case AssetData.State.IsError:
            //        newModel = Instantiate(_unityState.ErrorModel);
            //        break;
            //}
            //
            //if (newModel != null)
            //{
            //    Destroy(_currentModelObject);
            //
            //    newModel.transform.SetParent(transform);
            //    newModel.transform.localScale = Vector3.one;
            //    newModel.transform.localRotation = Quaternion.identity;
            //    newModel.transform.localPosition = Vector3.zero;
            //
            //    _currentModelObject = newModel;
            //
            //    if (newModel.TryGetComponent(out Animation animation))
            //        animation.enabled = false;
            //
            //    UpdateSelection(entity);
            //}
            //
            //if (scene.IsFloatingEntity(entity.Id)! == true)
            //{
            //    StaticUtilities.SetLayerRecursive(gameObject, LayerMask.NameToLayer("Ignore Raycast"));
            //}
        }

        private void UpdateDisplayedAsset(CommonAssetTypes.AssetInfo asset)
        {
            if (asset == displayedAsset) return; // already correct

            if (displayedAsset != null)
            {
                displayedAsset.assetFormatChanged -= UpdateModel;
            }

            if (asset != null)
            {
                asset.assetFormatChanged += UpdateModel;
            }

            displayedAsset = asset;

            UpdateModel();
        }

        private void UpdateModel()
        {
            if (displayedAsset == null)
            {
                DisplayNone();
                return;
            }

            var (availability, model) = discoveredAssets.GetAssetFormat<AssetFormatLoadedModel>(displayedAsset.assetId);

            switch (availability)
            {
                case DiscoveredAssets.AssetFormatAvailability.Available:
                    DisplayModel(model);
                    break;
                case DiscoveredAssets.AssetFormatAvailability.Loading:
                    DisplayLoading();
                    break;
                case DiscoveredAssets.AssetFormatAvailability.FormatError:
                case DiscoveredAssets.AssetFormatAvailability.FormatNotAvailable:
                case DiscoveredAssets.AssetFormatAvailability.DoesNotExist:
                    DisplayError();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void DisplayModel(AssetFormatLoadedModel model)
        {
            Assert.IsTrue(model.availability == CommonAssetTypes.Availability.Available);

            currentModelObject?.ReturnToPool();

            currentModelObject = model.CreateInstance();

            SetupTransform();
        }

        private void DisplayLoading()
        {
            currentModelObject?.ReturnToPool();

            currentModelObject = specialAssets.loadingModelProvider.CreateInstance();

            SetupTransform();
        }

        private void DisplayError()
        {
            currentModelObject?.ReturnToPool();

            currentModelObject = specialAssets.errorModelProvider.CreateInstance();

            SetupTransform();
        }

        private void DisplayNone()
        {
            currentModelObject?.ReturnToPool();
        }

        private void SetupTransform()
        {
            Assert.IsNotNull(currentModelObject);
            currentModelObject!.gameObject.transform.SetParent(transform);
            currentModelObject!.gameObject.transform.localPosition = Vector3.zero;
            currentModelObject!.gameObject.transform.localRotation = Quaternion.identity;
            currentModelObject!.gameObject.transform.localScale = Vector3.one;
        }

        public override void Deactivate()
        {
            DisplayNone();
            UpdateDisplayedAsset(null);
        }

        public class Factory : PlaceholderFactory<GltfShapeVisuals>
        {
        }
    }
}
