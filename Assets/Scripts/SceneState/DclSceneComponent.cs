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

        // The old definition where the property "scene" was called "sceneId" and of type "String".
        public static readonly ComponentDefinition oldSceneComponentDefinition =
            new ComponentDefinition(
                "Scene",
                "Scene",
                new DclComponentProperty.PropertyDefinition("sceneId", DclComponentProperty.PropertyType.String, ""));


        public DclSceneComponent(Guid sceneId) : base("Scene", "Scene")
        {
            Properties.Add(new DclComponentProperty<Guid>("scene", sceneId));
        }

        public DclSceneComponent(DclComponent dclComponent) : base(dclComponent.NameInCode, dclComponent.NameOfSlot)
        {
            this.Properties = dclComponent.Properties;
            if (!Validate())
            {
                if (ValidateOld())
                {
                    ConvertFromOldDefinition();
                    return;
                }
                throw new ArgumentException(nameof(dclComponent));
            }
        }

        public DclComponentProperty<Guid> sceneId => GetPropertyByName("scene")?.GetConcrete<Guid>();

        public bool Validate()
        {
            return IsFollowingDefinition(sceneComponentDefinition);
        }


        private bool ValidateOld() => IsFollowingDefinition(oldSceneComponentDefinition);
        /// <summary>
        /// Convert component that is in the old format into the new format.
        /// </summary>
        private void ConvertFromOldDefinition()
        {
            DclComponentProperty sceneIdProp = this.GetPropertyByName("sceneId");
            this.Properties.Remove(sceneIdProp);
            this.Properties.Add(new DclComponentProperty<Guid>("scene", Guid.Parse(sceneIdProp.GetConcrete<String>().FixedValue)));
        }
    }
}
