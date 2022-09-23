using Assets.Scripts.Visuals;
using Zenject;

public class EntityVisualsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindFactory<GltfShapeVisuals, GltfShapeVisuals.Factory>().FromNewComponentOn(gameObject);
        Container.BindFactory<PrimitiveShapeVisuals, PrimitiveShapeVisuals.Factory>().FromNewComponentOn(gameObject);
    }
}
