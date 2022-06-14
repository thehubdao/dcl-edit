using UnityEngine;

namespace Assets.Scripts.SceneState
{
    public class DclTransformComponent : DclComponent
    {
        public DclTransformComponent() : base("transform", "transform")
        {}

        public DclTransformComponent(DclComponent c) : base("transform", "transform")
        {
            Properties = c.Properties;
        }

        public DclComponentProperty<Vector3> Position => GetPropertyByName("position")?.GetConcrete<Vector3>();

        public DclComponentProperty<Quaternion> Rotation => GetPropertyByName("rotation")?.GetConcrete<Quaternion>();

        public DclComponentProperty<Vector3> Scale => GetPropertyByName("scale")?.GetConcrete<Vector3>();
        

        public bool Validate()
        {
            var posProperty = GetPropertyByName("position");

            if(posProperty == null)
                return false;

            if (posProperty.Type != DclComponentProperty.PropertyType.Vector3)
                return false;

            var quatProperty = GetPropertyByName("rotation");

            if (quatProperty == null)
                return false;

            if (quatProperty.Type != DclComponentProperty.PropertyType.Quaternion)
                return false;

            var scaleProperty = GetPropertyByName("scale");

            if (scaleProperty == null)
                return false;

            if (scaleProperty.Type != DclComponentProperty.PropertyType.Vector3)
                return false;

            return true;
        }
    }
}
