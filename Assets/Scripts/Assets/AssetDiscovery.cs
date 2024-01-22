using System.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Assets
{
    public class AssetDiscovery
    {
        // Dependencies
        private BuilderAssetDiscovery builderAssetDiscovery;
        private OnDiscAssetDiscovery onDiscAssetDiscovery;


        [Inject]
        private void Construct(
            BuilderAssetDiscovery builderAssetDiscovery,
            OnDiscAssetDiscovery onDiscAssetDiscovery)
        {
            this.builderAssetDiscovery = builderAssetDiscovery;
            this.onDiscAssetDiscovery = onDiscAssetDiscovery;
        }

        public void Initialize()
        {
            builderAssetDiscovery.Initialize();
            onDiscAssetDiscovery.Initialize();
        }
    }
}
