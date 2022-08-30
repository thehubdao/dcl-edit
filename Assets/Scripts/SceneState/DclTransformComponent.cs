using UnityEngine;
using UnityEngine.Android;

namespace Assets.Scripts.SceneState
{
    public class DclTransformComponent : DclComponent
    {
        public DclTransformComponent() : base("transform", "transform")
        { }

        public DclTransformComponent(DclComponent c) : base("transform", "transform")
        {
            Properties = c.Properties;
            Entity = c.Entity;
        }

        public DclComponentProperty<Vector3> Position => GetPropertyByName("position")?.GetConcrete<Vector3>();

        public DclComponentProperty<Quaternion> Rotation => GetPropertyByName("rotation")?.GetConcrete<Quaternion>();

        public DclComponentProperty<Vector3> Scale => GetPropertyByName("scale")?.GetConcrete<Vector3>();

        public Vector3 GlobalFixedPosition
        {
            get
            {
                if (Entity.Parent == null)
                {
                    return Position.FixedValue;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                var scaledLocalPosition = Position.FixedValue;
                scaledLocalPosition.Scale(parentTransform.Scale.FixedValue);
                var globalPosition = (parentTransform.GlobalFixedPosition + (parentTransform.GlobalFixedRotation * scaledLocalPosition));
                
                return globalPosition;
            }
        }
        public Vector3 GlobalPosition
        {
            get
            {
                if (Entity.Parent == null)
                {
                    return Position.Value;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                var scaledLocalPosition = Position.Value;
                scaledLocalPosition.Scale(parentTransform.Scale.Value);
                var globalPosition = (parentTransform.GlobalPosition + (parentTransform.GlobalRotation * scaledLocalPosition));
                
                return globalPosition;
            }
        }

        public Quaternion GlobalFixedRotation
        {
            get
            {
                if (Entity.Parent == null)
                {
                    return Rotation.FixedValue;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                return parentTransform.GlobalFixedRotation * Rotation.FixedValue;
            }
        }

        public Quaternion GlobalRotation
        {
            get
            {
                if (Entity.Parent == null)
                {
                    return Rotation.Value;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                return parentTransform.GlobalRotation * Rotation.Value;
            }
        }

        public bool Validate()
        {
            var posProperty = GetPropertyByName("position");

            if (posProperty == null)
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
