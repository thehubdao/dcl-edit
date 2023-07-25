using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.SceneState
{
    public class DclComponent
    {
        #region Property

        public abstract class DclComponentProperty
        {
            public class PropertyDefinition
            {
                [Flags]
                public enum Flags
                {
                    None = 0,
                    ParseInConstructor = 1 << 0,
                    ModelAssets = 1 << 1,
                    SceneAssets = 1 << 2,
                    // Reserved for assets: 1 << 3 - 7
                }

                public string name;
                public PropertyType type;
                public dynamic defaultValue;
                public Flags flags;

                public PropertyDefinition(string name, PropertyType type, dynamic defaultValue, params Flags[] flags)
                {
                    if (defaultValue == null)
                    {
                        throw new ArgumentException("Default value cannot be null");
                    }


                    this.name = name;
                    this.type = type;
                    this.defaultValue = defaultValue;
                    this.flags = flags.Aggregate(Flags.None, (left, right) => left | right);

                    if (!ValidateDefaultValue())
                    {
                        throw new ArgumentException($"The type of the default value ({defaultValue.GetType()}) does not mach the type of the property ({type})");
                    }
                }

                private bool ValidateDefaultValue()
                {
                    return type switch
                    {
                        PropertyType.None => false,
                        PropertyType.String => defaultValue is string,
                        PropertyType.Int => defaultValue is int,
                        PropertyType.Float => defaultValue is float,
                        PropertyType.Boolean => defaultValue is bool,
                        PropertyType.Vector3 => defaultValue is Vector3,
                        PropertyType.Quaternion => defaultValue is Quaternion,
                        PropertyType.Asset => defaultValue is Guid,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            public string PropertyName;

            public enum PropertyType
            {
                None,
                String,
                Int,
                Float,
                Boolean,
                Vector3,
                Quaternion,
                Asset
            }

            public PropertyType Type =>
                this switch
                {
                    DclComponentProperty<string> _ => PropertyType.String,
                    DclComponentProperty<int> _ => PropertyType.Int,
                    DclComponentProperty<float> _ => PropertyType.Float,
                    DclComponentProperty<bool> _ => PropertyType.Boolean,
                    DclComponentProperty<Vector3> _ => PropertyType.Vector3,
                    DclComponentProperty<Quaternion> _ => PropertyType.Quaternion,
                    DclComponentProperty<Guid> _ => PropertyType.Asset,
                    _ => PropertyType.None
                };

            public static DclComponentProperty NewFromType(PropertyType type, string name, dynamic initialValue)
            {
                return type switch
                {
                    PropertyType.None => throw new Exception("Can not make property of type none"),
                    PropertyType.String => new DclComponentProperty<string>(name, initialValue),
                    PropertyType.Int => new DclComponentProperty<int>(name, initialValue),
                    PropertyType.Float => new DclComponentProperty<float>(name, initialValue),
                    PropertyType.Boolean => new DclComponentProperty<bool>(name, initialValue),
                    PropertyType.Vector3 => new DclComponentProperty<Vector3>(name, initialValue),
                    PropertyType.Quaternion => new DclComponentProperty<Quaternion>(name, initialValue),
                    PropertyType.Asset => new DclComponentProperty<Guid>(name, initialValue),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
            }

            public DclComponentProperty<T> GetConcrete<T>()
            {
                if (!(this is DclComponentProperty<T> dclComponentProperty))
                {
                    throw new System.Exception("Property is not of type " + typeof(T).Name);
                }

                return dclComponentProperty;
            }

            public DclComponentProperty DeepCopy(DclComponentProperty other)
            {
                Type propertyType = other.GetType();

                if (propertyType == typeof(DclComponentProperty<Vector3>))
                {
                    return new DclComponentProperty<Vector3>(other.PropertyName, other.GetConcrete<Vector3>().Value);
                }
                else if (propertyType == typeof(DclComponentProperty<Quaternion>))
                {
                    return new DclComponentProperty<Quaternion>(other.PropertyName, other.GetConcrete<Quaternion>().Value);
                }
                else if (propertyType == typeof(DclComponentProperty<Guid>))
                {
                    return new DclComponentProperty<Guid>(other.PropertyName, other.GetConcrete<Guid>().Value);
                }
                else if (propertyType == typeof(DclComponentProperty<string>))
                {
                    return new DclComponentProperty<string>(other.PropertyName, other.GetConcrete<string>().Value);
                }
                else if (propertyType == typeof(DclComponentProperty<int>))
                {
                    return new DclComponentProperty<int>(other.PropertyName, other.GetConcrete<int>().Value);
                }
                else if (propertyType == typeof(DclComponentProperty<float>))
                {
                    return new DclComponentProperty<float>(other.PropertyName, other.GetConcrete<float>().Value);
                }
                else if (propertyType == typeof(DclComponentProperty<bool>))
                {
                    return new DclComponentProperty<bool>(other.PropertyName, other.GetConcrete<bool>().Value);
                }

                return other;
            }

            protected bool _isFloating;


            /**
             * <summary>Is the value currently floating</summary>>
             */
            public bool IsFloating => _isFloating;

            /**
             * <summary>Reset the floating value</summary>
             */
            public void ResetFloating()
            {
                _isFloating = false;
            }

            public bool IsFollowingDefinition(PropertyDefinition definition)
            {
                return
                    definition.name == PropertyName &&
                    definition.type == Type;
            }
        }

        public class DclComponentProperty<T> : DclComponentProperty
        {
            private Subscribable<T> _fixedValue = new();
            private Subscribable<T> _floatingValue = new();
            //private T _fixedValue;
            //private T _floatingValue;
            public Subscribable<T> _Value => _isFloating ? _floatingValue : _fixedValue;
            
            /**
             * <summary>Constructor with an initial value</summary>
             * <param name="name"></param>
             * <param name="initialValue">The initial fixed value</param>
             */
            public DclComponentProperty(string name, T initialValue)
            {
                PropertyName = name;
                _fixedValue.Value = initialValue;
                _floatingValue.Value = initialValue;
                _isFloating = false;
            }

            // Value stuff


            /**
             * <summary>The current value. When value is floating, returns the floating value</summary>
             */
            public T Value => _isFloating ? _floatingValue.Value : _fixedValue.Value;

            /**
             * <summary>Get fixed value. All ways returns fixed value, even if floating value is available</summary>
             */
            public T FixedValue => _fixedValue.Value;

            /**
             * <summary>Set floating value</summary>
             * <param name="value">Value to set</param>
             */
            public void SetFloatingValue(T value)
            {
                _floatingValue.Value = value;
                _isFloating = true;
            }

            /**
             * <summary>Set fixed value</summary>
             * <remarks>Should only be called from Commands</remarks>
             * <param name="value">Value to set</param>
             */
            public void SetFixedValue(T value)
            {
                _fixedValue.Value = value;
                ResetFloating();
            }
        }

        #endregion // Property

        #region Definition

        public class ComponentDefinition
        {
            public string NameInCode { get; }

            public string NameOfSlot { get; }

            [CanBeNull]
            public string SourceFile { get; }

            public List<DclComponentProperty.PropertyDefinition> properties;

            public ComponentDefinition(string nameInCode, string nameOfSlot, [CanBeNull] string sourceFile = null, params DclComponentProperty.PropertyDefinition[] properties)
            {
                NameInCode = nameInCode;
                NameOfSlot = nameOfSlot;
                SourceFile = sourceFile;
                this.properties = properties.ToList();
            }

            public DclComponentProperty.PropertyDefinition GetPropertyDefinitionByName(string name)
            {
                return properties.Find(pd => pd.name == name);
            }
        }

        #endregion // Definition

        public DclEntity Entity = null;

        public string NameInCode { get; }

        public string NameOfSlot { get; }

        public List<DclComponentProperty> Properties = new List<DclComponentProperty>();

        public DclComponentProperty GetPropertyByName(string name)
        {
            return Properties.Exists(p => p.PropertyName == name) ?
                Properties.Find(p => p.PropertyName == name) :
                null;
        }

        public DclComponent DeepCopy()
        {
            DclComponent deepcopyComponent = new DclComponent(NameInCode, NameOfSlot);
            deepcopyComponent.Properties.Clear();
            foreach (var prop in Properties)
            {
                DclComponentProperty componentProperty = prop.DeepCopy(prop);
                deepcopyComponent.Properties.Add(componentProperty);
            }

            return deepcopyComponent;
        }

        public DclComponent(ComponentDefinition definition)
        {
            NameInCode = definition.NameInCode;
            NameOfSlot = definition.NameOfSlot;

            foreach (var propertyDefinition in definition.properties)
            {
                DclComponentProperty property = DclComponentProperty.NewFromType(propertyDefinition.type, propertyDefinition.name, propertyDefinition.defaultValue);

                Properties.Add(property);
            }
        }

        public DclComponent(string name, string slotName)
        {
            NameInCode = name;
            NameOfSlot = slotName;
        }

        public bool IsFollowingDefinition(ComponentDefinition definition)
        {
            if (definition.NameInCode != NameInCode)
            {
                return false;
            }

            if (definition.NameOfSlot != NameOfSlot)
            {
                return false;
            }

            if (definition.properties.Count != Properties.Count)
            {
                return false;
            }

            for (var i = 0; i < Properties.Count; i++)
            {
                if (!Properties[i].IsFollowingDefinition(definition.properties[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}



