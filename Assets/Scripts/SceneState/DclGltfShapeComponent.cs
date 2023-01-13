using Assets.Scripts.SceneState;
using System;

public class DclGltfShapeComponent : DclComponent
{
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
        if (NameInCode != "GLTFShape")
            return false;

        if (NameOfSlot != "Shape")
            return false;

        var assetProp = GetPropertyByName("asset");

        if (assetProp == null)
            return false;

        if (assetProp.Type != DclComponentProperty.PropertyType.Asset)
            return false;

        var visibleProp = GetPropertyByName("visible");

        if (visibleProp == null)
            return false;

        if (visibleProp.Type != DclComponentProperty.PropertyType.Boolean)
            return false;

        var withColProp = GetPropertyByName("withCollisions");

        if (withColProp == null)
            return false;

        if (withColProp.Type != DclComponentProperty.PropertyType.Boolean)
            return false;

        var isPtrBlckrProp = GetPropertyByName("isPointerBlocker");

        if (isPtrBlckrProp == null)
            return false;

        if (isPtrBlckrProp.Type != DclComponentProperty.PropertyType.Boolean)
            return false;

        return true;
    }
}
