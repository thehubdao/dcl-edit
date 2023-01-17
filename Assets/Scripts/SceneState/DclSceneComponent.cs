using System;

namespace Assets.Scripts.SceneState
{
    public class DclSceneComponent : DclComponent
    {
        public static readonly ComponentDefinition sceneComponentDefinition =
            new ComponentDefinition(
                "Scene",
                "Scene",
                new DclComponentProperty.PropertyDefinition("sceneId", DclComponentProperty.PropertyType.String, ""));


        public DclSceneComponent(Guid sceneId) : base("Scene", "Scene")
        {
            Properties.Add(new DclComponentProperty<string>("sceneId", sceneId.ToString() ?? ""));
        }

        public DclSceneComponent(DclComponent dclComponent) : base(dclComponent.NameInCode, dclComponent.NameOfSlot)
        {
            this.Properties = dclComponent.Properties;
            if (!Validate())
            {
                throw new ArgumentException(nameof(dclComponent));
            }
        }

        public DclComponentProperty<string> sceneId => GetPropertyByName("sceneId")?.GetConcrete<string>();

        public bool Validate()
        {
            return IsFollowingDefinition(sceneComponentDefinition);
        }
    }
}
