using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Assets
{
    public class CommonAssetTypes
    {
        public class AssetInfo
        {
            public Guid assetId;
            public string assetName;
            public AssetType assetType;
            public AssetSource assetSource;
            public string displayPath;
            public AssetFormat baseFormat;
            public List<AssetFormat> availableFormats;
        }

        public enum Availability
        {
            Unavailable,
            Loading,
            Available,
            Error
        }

        public enum AssetType
        {
            Unknown,
            Model3D,
            Image,
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
    }
}