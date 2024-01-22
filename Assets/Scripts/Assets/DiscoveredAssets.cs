using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Assets
{
    public class DiscoveredAssets
    {
        // Dependencies
        private AssetFormatTransformer assetFormatTransformer;

        [Inject]
        private void Construct(AssetFormatTransformer assetFormatTransformer)
        {
            this.assetFormatTransformer = assetFormatTransformer;
        }


        public readonly SubscribableDictionary<Guid, CommonAssetTypes.AssetInfo> discoveredAssets = new();

        public enum AssetFormatAvailability
        {
            Available,
            Loading,
            FormatError,
            FormatNotAvailable,
            DoesNotExist
        }

        public (AssetFormatAvailability, T) GetAssetFormat<T>(Guid assetId) where T : CommonAssetTypes.AssetFormat
        {
            // Step 1: Does the asset exist at all
            if (!discoveredAssets.TryGetValue(assetId, out var info))
            {
                return (AssetFormatAvailability.DoesNotExist, null);
            }

            // Step 2: Does the Format exist
            var wantedFormatType = typeof(T);
            var wantedFormat = info.GetAssetFormatOrNull(wantedFormatType);

            if (wantedFormat != null)
            {
                // check if the found format is up to date
                if (wantedFormat.hash != info.baseFormat.hash)
                {
                    info.PurgeOutOfDateFormats();
                }
                else
                {
                    return wantedFormat.availability switch
                    {
                        // Step 2a: The format is available
                        CommonAssetTypes.Availability.Available =>
                            (AssetFormatAvailability.Available, (T) wantedFormat),

                        // Step 2b: The format is still loading
                        CommonAssetTypes.Availability.Loading =>
                            (AssetFormatAvailability.Loading, null),

                        // Step 2c: The format has an error
                        CommonAssetTypes.Availability.Error =>
                            (AssetFormatAvailability.FormatError, null),

                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            // Step 3: create the missing format(s)
            try
            {
                var transformationResult = assetFormatTransformer.TransformToFormat(info, wantedFormatType);

                return transformationResult switch
                {
                    // Step 3a: Missing format has been instantly created
                    AssetFormatTransformer.TransformToFormatReturn.Available =>
                        (AssetFormatAvailability.Available, (T) info.GetAssetFormatOrNull(wantedFormatType)),

                    // Step 3b: Missing format is loading
                    AssetFormatTransformer.TransformToFormatReturn.Loading =>
                        (AssetFormatAvailability.Loading, null),

                    // Step 3c: Missing format can not be created
                    AssetFormatTransformer.TransformToFormatReturn.Impossible =>
                        (AssetFormatAvailability.FormatNotAvailable, null),

                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}


