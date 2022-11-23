using UnityEngine;

namespace Assets.Scripts.SceneState
{
    public class DclTransformComponent : DclComponent
    {
        public DclTransformComponent(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null) : base("Transform", "Transform")
        {
            Properties.Add(new DclComponentProperty<Vector3>("position", position ?? Vector3.zero));
            Properties.Add(new DclComponentProperty<Quaternion>("rotation", rotation ?? Quaternion.identity));
            Properties.Add(new DclComponentProperty<Vector3>("scale", scale ?? Vector3.one));
        }

        public DclTransformComponent(DclComponent c) : base(c.NameInCode, c.NameInCode)
        {
            Properties = c.Properties;
            Entity = c.Entity;
        }

        public DclComponentProperty<Vector3> Position => GetPropertyByName("position")?.GetConcrete<Vector3>();

        public DclComponentProperty<Quaternion> Rotation => GetPropertyByName("rotation")?.GetConcrete<Quaternion>();

        public DclComponentProperty<Vector3> Scale => GetPropertyByName("scale")?.GetConcrete<Vector3>();

        // gives the global fixed position
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
            if (NameInCode != "Transform")
                return false;

            if (NameOfSlot != "Transform")
                return false;

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

        /// <summary>
        /// Transforms position from world space to local space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 InverseTransformPoint(Vector3 position)
        {
            var localPos = Quaternion.Inverse(GlobalRotation) * (position - GlobalPosition);

            var invertedScale = Vector3.zero;
            invertedScale.x = 1 / Scale.Value.x;
            invertedScale.y = 1 / Scale.Value.y;
            invertedScale.z = 1 / Scale.Value.z;

            localPos = Vector3.Scale(localPos, invertedScale);
            return localPos;
        }
        /// <summary>
        /// Transforms position from local space to world space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 TransformPoint(Vector3 position) => GlobalPosition + GlobalRotation * Vector3.Scale(position, Scale.Value);
    }
}
