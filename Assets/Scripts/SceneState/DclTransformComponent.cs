using UnityEngine;

namespace Assets.Scripts.SceneState
{
    public class DclTransformComponent : DclComponent
    {
        public static readonly ComponentDefinition transformComponentDefinition =
            new ComponentDefinition(
                "Transform",
                "Transform",
                false,
                null,
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

        public DclComponentProperty<Vector3> position => GetPropertyByName("position")?.GetConcrete<Vector3>();

        public DclComponentProperty<Quaternion> rotation => GetPropertyByName("rotation")?.GetConcrete<Quaternion>();

        public DclComponentProperty<Vector3> scale => GetPropertyByName("scale")?.GetConcrete<Vector3>();

        public Matrix4x4 globalTransformMatrix
        {
            get
            {
                if (Entity.Parent == null)
                {
                    Matrix4x4 transformMatrix = new Matrix4x4();
                    transformMatrix.SetTRS(position.Value, rotation.Value, scale.Value);
                    return transformMatrix;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                var parentMatrix = parentTransform.globalTransformMatrix;

                Matrix4x4 localMatrix = new Matrix4x4();
                localMatrix.SetTRS(position.Value, rotation.Value, scale.Value);

                var globalMatrix = parentMatrix * localMatrix;

                return globalMatrix;
            }
        }

        public Vector3 globalPosition
        {
            get
            {
                Vector3 position;
                position.x = globalTransformMatrix.m03;
                position.y = globalTransformMatrix.m13;
                position.z = globalTransformMatrix.m23;
                position /= globalTransformMatrix.m33;

                return position;
            }
            set
            {
                if (Entity.Parent != null)
                {
                    position.SetFloatingValue(Entity.Parent.GetTransformComponent().InverseTransformPoint(value));
                }
                else
                {
                    position.SetFloatingValue(value);
                }
            }
        }

        public Quaternion globalRotation
        {
            get => Entity.Parent == null ? rotation.Value : Entity.Parent.GetTransformComponent().globalRotation * rotation.Value;
            set
            {
                if (Entity.Parent == null)
                {
                    rotation.SetFloatingValue(value);
                }
                else
                {
                    rotation.SetFloatingValue(Quaternion.Inverse(Entity.Parent.GetTransformComponent().globalRotation) * value);
                }
            }
        }

        public Matrix4x4 globalFixedTransformMatrix
        {
            get
            {
                if (Entity.Parent == null)
                {
                    Matrix4x4 transformMatrix = new Matrix4x4();
                    transformMatrix.SetTRS(position.FixedValue, rotation.FixedValue, scale.FixedValue);
                    return transformMatrix;
                }

                var parentTransform = Entity.Parent.GetTransformComponent();
                var parentMatrix = parentTransform.globalFixedTransformMatrix;

                Matrix4x4 localMatrix = new Matrix4x4();
                localMatrix.SetTRS(position.FixedValue, rotation.FixedValue, scale.FixedValue);

                var globalMatrix = parentMatrix * localMatrix;

                return globalMatrix;
            }
        }

        public Vector3 globalFixedPosition
        {
            get
            {
                Vector3 position;
                position.x = globalFixedTransformMatrix.m03;
                position.y = globalFixedTransformMatrix.m13;
                position.z = globalFixedTransformMatrix.m23;
                position /= globalFixedTransformMatrix.m33;

                return position;
            }
            set
            {
                if (Entity.Parent != null)
                {
                    position.SetFixedValue(Entity.Parent.GetTransformComponent().InverseTransformPoint(value));
                }
                else
                {
                    position.SetFixedValue(value);
                }
            }
        }

        public Quaternion globalFixedRotation
        {
            get
            {
                 return Entity.Parent == null ? rotation.FixedValue : Entity.Parent.GetTransformComponent().globalFixedRotation* rotation.FixedValue;
            }
            set
            {
                if (Entity.Parent == null)
                {
                    rotation.SetFixedValue(value);
                }
                else
                {
                    rotation.SetFixedValue(Quaternion.Inverse(Entity.Parent.GetTransformComponent().globalRotation) * value);
                }
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
            return globalTransformMatrix.inverse.MultiplyPoint(position);
        }
    }
}
