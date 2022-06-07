using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ProjectState
{
    public class DclAssets
    {
        public Dictionary<Guid, DclAsset> UsedAssets = new Dictionary<Guid, DclAsset>();

        public DclAsset GetAssetById(Guid id)
        {
            return UsedAssets.ContainsKey(id) ? UsedAssets[id] : null;
        }
    }
}
