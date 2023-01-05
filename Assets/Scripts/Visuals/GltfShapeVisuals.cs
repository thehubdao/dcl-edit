using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GltfShapeVisuals : ShapeVisuals, IDisposable
    {
        private GameObject _currentModelObject = null;
        private DclEntity entity;

        // Dependencies
        private EntityVisuals _entityVisuals;
        private AssetManagerSystem _assetManagerSystem;
        private UnityState _unityState;
        private EditorEvents _editorEvents;
        private SceneManagerSystem _sceneManagerSystem;

        [Inject]
        private void Construct(
            EntityVisuals entityVisuals,
            AssetManagerSystem assetManagerSystem,
            UnityState unityState,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            _entityVisuals = entityVisuals;
            _assetManagerSystem = assetManagerSystem;
            _unityState = unityState;
            _editorEvents = editorEvents;
            _sceneManagerSystem = sceneManagerSystem;

            entity = _sceneManagerSystem.GetCurrentScene()?.GetEntityById(_entityVisuals.id);
            _editorEvents.onAssetDataUpdatedEvent += OnAssetDataUpdatedCallback;
        }

        void IDisposable.Dispose()
        {
            _editorEvents.onAssetDataUpdatedEvent -= OnAssetDataUpdatedCallback;
        }

        public void OnAssetDataUpdatedCallback(List<Guid> ids)
        {
            var assetId = entity?.GetComponentByName("GLTFShape")?.GetPropertyByName("asset")?.GetConcrete<Guid>()?.Value;
            if (assetId.HasValue && ids.Contains(assetId.Value))
            {
                UpdateVisuals(entity);
            }
        }

        public override void UpdateVisuals(DclEntity entity)
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
                        ModelAssetData modelData = (ModelAssetData)data;
                        if (modelData.data == null)
                            return;

                        newModel = modelData.data;
                    }
                    break;
                case AssetData.State.IsLoading:

                    break;
                case AssetData.State.IsError:
                    newModel = Instantiate(_unityState.ErrorModel);
                    break;
                default:
                    break;
            }

            if (newModel)
            {
                Destroy(_currentModelObject);

                newModel.transform.SetParent(transform);
                newModel.transform.localScale = Vector3.one;
                newModel.transform.localRotation = Quaternion.identity;
                newModel.transform.localPosition = Vector3.zero;

                _currentModelObject = newModel;

                UpdateSelection(entity);
            }

            if (_sceneManagerSystem.GetCurrentScene()?.IsFloatingEntity(entity.Id)! == true)
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
