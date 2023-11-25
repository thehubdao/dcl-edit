using System.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Assets
{
    public class AssetDiscovery
    {
        // Dependencies
        private DiscoveredAssets discoveredAssets;
        private BuilderAssetDiscovery builderAssetDiscovery;

        [Inject]
        private void Construct(
            DiscoveredAssets discoveredAssets,
            BuilderAssetDiscovery builderAssetDiscovery)
        {
            this.discoveredAssets = discoveredAssets;
            this.builderAssetDiscovery = builderAssetDiscovery;
        }

        public void Initialize()
        {
            builderAssetDiscovery.Initialize();
        }
    }
}
