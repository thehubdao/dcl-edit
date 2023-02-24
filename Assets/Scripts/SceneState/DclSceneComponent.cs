using System;

namespace Assets.Scripts.SceneState
{
    public class DclSceneComponent : DclComponent
    {
        public static readonly ComponentDefinition sceneComponentDefinition =
            new ComponentDefinition(
                "Scene",
                "Scene",
                new DclComponentProperty.PropertyDefinition("scene", DclComponentProperty.PropertyType.Asset, Guid.Empty));

        public DclSceneComponent(Guid sceneId) : base("Scene", "Scene")
        {
            Properties.Add(new DclComponentProperty<Guid>("scene", sceneId));
        }

        public DclSceneComponent(DclComponent dclComponent) : base(dclComponent.NameInCode, dclComponent.NameOfSlot)
        {
            this.Properties = dclComponent.Properties;
            if (!Validate()) throw new ArgumentException(nameof(dclComponent));
        }

        public DclComponentProperty<Guid> sceneId => GetPropertyByName("scene")?.GetConcrete<Guid>();

        public bool Validate()
        {
            return IsFollowingDefinition(sceneComponentDefinition);
        }
    }
}
