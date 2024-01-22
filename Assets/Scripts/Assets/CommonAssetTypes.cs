using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Assets
{
    public class CommonAssetTypes
    {
        public class AssetInfo
        {
            public Guid assetId;
            public string assetName;
            public bool visible;
            public AssetType assetType;
            public AssetSource assetSource;
            public string displayPath;
            public AssetFormat baseFormat;
            public List<AssetFormat> availableFormats;
            public List<AssetInfo> dependencies;
            public event Action assetFormatChanged;


            public void InvokeAssetFormatChanged()
            {
                assetFormatChanged?.Invoke();
            }

            [CanBeNull]
            public AssetFormat GetAssetFormatOrNull(Type assetFormatType)
            {
                return availableFormats.Find(af => af.GetType() == assetFormatType);
            }

            public void PurgeOutOfDateFormats()
            {
                var baseHash = baseFormat.hash;
                var newAvailableFormats = availableFormats.Where(f => f.hash == baseHash).ToList();
                availableFormats = newAvailableFormats;
            }
        }

        public enum Availability
        {
            Available,
            Loading,
            Error
        }

        public enum AssetType
        {
            Unknown,
            Model3D,
            Image,
            Scene,
            Entity
        }

        public enum AssetSource
        {
            Unknown,
            DecentralandBuilder,
            Local
        }

        public abstract class AssetFormat
        {
            public abstract string formatName { get; }
            public abstract string hash { get; }
            public abstract Availability availability { get; }
        }

        public interface IModelProvider
        {
            GameObjectInstance CreateInstance();
            void ReturnToPool(GameObjectInstance modelInstance);
        }

        public class GameObjectInstance
        {
            public GameObject gameObject;

            private readonly CommonAssetTypes.IModelProvider parentModelProvider;

            public void ReturnToPool()
            {
                parentModelProvider.ReturnToPool(this);
            }

            public GameObjectInstance(GameObject gameObject, CommonAssetTypes.IModelProvider parentModelProvider)
            {
                this.gameObject = gameObject;
                this.parentModelProvider = parentModelProvider;
            }
        }

        public class GameObjectPoolEntry
        {
            public GameObjectInstance modelInstance;

            public GameObjectPoolEntry(GameObjectInstance modelInstance)
            {
                this.modelInstance = modelInstance;
            }
        }
    }
}