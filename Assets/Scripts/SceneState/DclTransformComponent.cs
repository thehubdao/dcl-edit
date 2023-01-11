using UnityEngine;

namespace Assets.Scripts.SceneState
{
    public class DclTransformComponent : DclComponent
    {
        public static readonly ComponentDefinition transformComponentDefinition =
            new ComponentDefinition(
                "Transform",
                "Transform",
                new DclComponentProperty.PropertyDefinition("position", DclComponentProperty.PropertyType.Vector3, Vector3.zero),
                new DclComponentProperty.PropertyDefinition("rotation", DclComponentProperty.PropertyType.Quaternion, Quaternion.identity),
                new DclComponentProperty.PropertyDefinition("scale", DclComponentProperty.PropertyType.Vector3, Vector3.one));

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

        public Matrix4x4 GlobalTransformMatrix
        {
            get
            {
                if (Entity.Parent == null)
                {
                    Matrix4x4 transformMatrix = new Matrix4x4();
                    transformMatrix.SetTRS(Position.Value, Rotation.Value, Scale.Value);
                    return transformMatrix;
                }
                
                var parentTransform = Entity.Parent.GetTransformComponent();
                var parentMatrix = parentTransform.GlobalTransformMatrix;

                Matrix4x4 localMatrix = new Matrix4x4();
                localMatrix.SetTRS(Position.Value, Rotation.Value, Scale.Value);

                var globalMatrix = parentMatrix * localMatrix;

                return globalMatrix;
            }
        }

        public Vector3 GlobalPosition
        {
            get
            {
                Vector3 position;
                position.x = GlobalTransformMatrix.m03;
                position.y = GlobalTransformMatrix.m13;
                position.z = GlobalTransformMatrix.m23;
                position /= GlobalTransformMatrix.m33;

                return position;
            }
        }
        public Quaternion GlobalRotation
        {
            get
            {
                return Entity.Parent == null
                    ? Rotation.Value
                    : Entity.Parent.GetTransformComponent().GlobalRotation * Rotation.Value;
            }
        }
        public Matrix4x4 GlobalFixedTransformMatrix
        {
            get
            {
                if (Entity.Parent == null)
                {
                    Matrix4x4 transformMatrix = new Matrix4x4();
                    transformMatrix.SetTRS(Position.FixedValue, Rotation.FixedValue, Scale.FixedValue);
                    return transformMatrix;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                var parentMatrix = parentTransform.GlobalFixedTransformMatrix;

                Matrix4x4 localMatrix = new Matrix4x4();
                localMatrix.SetTRS(Position.FixedValue, Rotation.FixedValue, Scale.FixedValue);

                var globalMatrix = parentMatrix * localMatrix;

                return globalMatrix;
            }
        }

        public Vector3 GlobalFixedPosition
        {
            get
            {
                Vector3 position;
                position.x = GlobalFixedTransformMatrix.m03;
                position.y = GlobalFixedTransformMatrix.m13;
                position.z = GlobalFixedTransformMatrix.m23;
                position /= GlobalFixedTransformMatrix.m33;

                return position;
            }
        }

        public Quaternion GlobalFixedRotation
        {
            get
            {
                return GlobalFixedTransformMatrix.rotation;
            }
        }

        public bool Validate()
        {
            return IsFollowingDefinition(transformComponentDefinition);
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
