using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class AssetDialogHandler : MonoBehaviour
    {
        public UiAssetBrowserVisuals assetBrowserVisuals;

        // Dependencies
        DialogSystem dialogSystem;

        [Inject]
        void Construct(DialogSystem dialogSystem)
        {
            this.dialogSystem = dialogSystem;
        }


        public void CloseDialog() => dialogSystem.CloseCurrentDialog();
    }
}