using Assets.Scripts.SceneState;
using System;
using UnityEngine;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty.PropertyDefinition.Flags;

public class DclGltfShapeComponent : DclComponent
{
    public static readonly ComponentDefinition gltfShapeComponentDefinition =
        new ComponentDefinition(
            "GLTFShape",
            "Shape",
            true,
            null,
            new DclComponentProperty.PropertyDefinition("asset", DclComponentProperty.PropertyType.Asset, Guid.Empty, ParseInConstructor),
            new DclComponentProperty.PropertyDefinition("visible", DclComponentProperty.PropertyType.Boolean, true),
            new DclComponentProperty.PropertyDefinition("withCollisions", DclComponentProperty.PropertyType.Boolean, true),
            new DclComponentProperty.PropertyDefinition("isPointerBlocker", DclComponentProperty.PropertyType.Boolean, true)
            );

    public DclGltfShapeComponent(Guid assetId, bool visible = true, bool withCollisions = true, bool isPointerBlocker = true) : base("GLTFShape", "Shape")
    {
        Properties.Add(new DclComponentProperty<Guid>("asset", assetId));
        Properties.Add(new DclComponentProperty<bool>("visible", visible));
        Properties.Add(new DclComponentProperty<bool>("withCollisions", withCollisions));
        Properties.Add(new DclComponentProperty<bool>("isPointerBlocker", isPointerBlocker));
    }

    public DclGltfShapeComponent(DclComponent c) : base(c.NameInCode, c.NameInCode)
    {
        Properties = c.Properties;
        Entity = c.Entity;
    }

    public DclComponentProperty<Guid> Asset => GetPropertyByName("asset")?.GetConcrete<Guid>();
    public DclComponentProperty<bool> Visible => GetPropertyByName("visible")?.GetConcrete<bool>();
    public DclComponentProperty<bool> WithCollisions => GetPropertyByName("withCollisions")?.GetConcrete<bool>();
    public DclComponentProperty<bool> IsPointerBlocker => GetPropertyByName("isPointerBlocker")?.GetConcrete<bool>();

    public bool Validate()
    {
        return IsFollowingDefinition(gltfShapeComponentDefinition);
    }
}
