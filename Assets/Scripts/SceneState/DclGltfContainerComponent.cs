using Assets.Scripts.SceneState;
using System;
using UnityEngine;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty.PropertyDefinition.Flags;

public class DclGltfContainerComponent : DclComponent
{
    public static readonly ComponentDefinition gltfShapeComponentDefinition =
        new ComponentDefinition(
            "GltfContainer",
            "Renderer",
            true,
            null,
            new DclComponentProperty.PropertyDefinition("src", DclComponentProperty.PropertyType.Asset, Guid.Empty, ParseInConstructor),
            new DclComponentProperty.PropertyDefinition("color", DclComponentProperty.PropertyType.Color, new Color(0, 0, 0))
            );

    public DclGltfContainerComponent(Guid assetId, Color color) : base(gltfShapeComponentDefinition.NameInCode, gltfShapeComponentDefinition.NameOfSlot)
    {
        Properties.Add(new DclComponentProperty<Guid>("src", assetId));
        Properties.Add(new DclComponentProperty<Color>("color", color));
    }

    public DclGltfContainerComponent(DclComponent c) : base(c.NameInCode, c.NameInCode)
    {
        Properties = c.Properties;
        Entity = c.Entity;
    }

    public DclComponentProperty<Guid> Src => GetPropertyByName("src")?.GetConcrete<Guid>();
    public DclComponentProperty<Color> Color => GetPropertyByName("color")?.GetConcrete<Color>();

    public bool Validate()
    {
        return IsFollowingDefinition(gltfShapeComponentDefinition);
    }
}
