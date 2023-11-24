using Assets.Scripts.EditorState;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    //public class AssetThumbnailManagerSystem
    //{
    //    // Dependencies
    //    //private AssetThumbnailGeneratorSystem _assetThumbnailGeneratorSystem;
    //    private IAssetLoaderSystem[] assetLoaderSystems;
    //
    //    [Inject]
    //    public void Construct( /*AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem,*/ params IAssetLoaderSystem[] assetLoaderSystems)
    //    {
    //        //_assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
    //        this.assetLoaderSystems = assetLoaderSystems;
    //    }
    //
    //    public AssetThumbnail GetThumbnailById(Guid id)
    //    {
    //        foreach (var loader in assetLoaderSystems)
    //        {
    //            var thumbnail = loader.GetThumbnailById(id);
    //            if (thumbnail != null)
    //            {
    //                return thumbnail;
    //            }
    //        }
    //
    //        return new AssetThumbnail(id, AssetData.State.IsError, null);
    //        //_assetThumbnailGeneratorSystem.Enqueue(id);
    //    }
    //
    //    //public void SetThumbnailById(Guid id, Texture2D newThumbnail)
    //    //{
    //    //    foreach (IAssetLoaderSystem loaderSystem in _assetLoaderSystems)
    //    //    {
    //    //        loaderSystem.SetThumbnailById(id, newThumbnail);
    //    //    }
    //    //}
    //}
}