using Assets.Scripts.SceneState;
using System;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty.PropertyDefinition.Flags;

public class DclGltfContainerComponent : DclComponent
{
    public static readonly ComponentDefinition gltfShapeComponentDefinition =
        new ComponentDefinition(
            "GltfContainer",
            "Renderer",
            true,
            null,
            new DclComponentProperty.PropertyDefinition("src", DclComponentProperty.PropertyType.Asset, Guid.Empty, ParseInConstructor));

    public DclGltfContainerComponent(Guid assetId) : base(gltfShapeComponentDefinition.NameInCode, gltfShapeComponentDefinition.NameOfSlot)
    {
        Properties.Add(new DclComponentProperty<Guid>("src", assetId));
    }

    public DclGltfContainerComponent(DclComponent c) : base(c.NameInCode, c.NameInCode)
    {
        Properties = c.Properties;
        Entity = c.Entity;
    }

    public DclComponentProperty<Guid> Src => GetPropertyByName("src")?.GetConcrete<Guid>();

    public bool Validate()
    {
        return IsFollowingDefinition(gltfShapeComponentDefinition);
    }
}
