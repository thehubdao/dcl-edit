using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json.Linq;
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
                public string name;
                public PropertyType type;
                public dynamic defaultValue;

                public PropertyDefinition(string name, PropertyType type, dynamic defaultValue)
                {
                    if(defaultValue == null){
                        throw new ArgumentException("Default value cannot be null");
                    }

                    this.name = name;
                    this.type = type;
                    this.defaultValue = defaultValue;

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
        }

        public class DclComponentProperty<T> : DclComponentProperty
        {
            private T _fixedValue;
            private T _floatingValue;

            /**
             * <summary>Constructor with an initial value</summary>
             * <param name="name"></param>
             * <param name="initialValue">The initial fixed value</param>
             */
            public DclComponentProperty(string name, T initialValue)
            {
                PropertyName = name;
                _fixedValue = initialValue;
                _floatingValue = initialValue;
                _isFloating = false;
            }

            // Value stuff


            /**
             * <summary>The current value. When value is floating, returns the floating value</summary>
             */
            public T Value => _isFloating ? _floatingValue : _fixedValue;

            /**
             * <summary>Get fixed value. All ways returns fixed value, even if floating value is available</summary>
             */
            public T FixedValue => _fixedValue;

            /**
             * <summary>Set floating value</summary>
             * <param name="value">Value to set</param>
             */
            public void SetFloatingValue(T value)
            {
                _floatingValue = value;
                _isFloating = true;
            }

            /**
             * <summary>Set fixed value</summary>
             * <remarks>Should only be called from Commands</remarks>
             * <param name="value">Value to set</param>
             */
            public void SetFixedValue(T value)
            {
                _fixedValue = value;
                ResetFloating();
            }
        }

        #endregion

        #region Definition

        public class ComponentDefinition
        {
            public string NameInCode { get; }

            public string NameOfSlot { get; }

            public List<DclComponentProperty.PropertyDefinition> properties;

            public ComponentDefinition(string nameInCode, string nameOfSlot, params DclComponentProperty.PropertyDefinition[] properties)
            {
                NameInCode = nameInCode;
                NameOfSlot = nameOfSlot;
                this.properties = properties.ToList();
            }
        }

        #endregion

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

            //foreach (var property in properties)
            //{
            //    DclComponentProperty p;
            //
            //    // switch doesn't work with the Type type
            //    if (property.Key == typeof(string))
            //    {
            //        p = new DclComponentProperty<string>(property.Value, "");
            //    }
            //    else if (property.Key == typeof(int))
            //    {
            //        p = new DclComponentProperty<int>(property.Value, 0);
            //    }
            //    else if (property.Key == typeof(float))
            //    {
            //        p = new DclComponentProperty<float>(property.Value, 0.0f);
            //    }
            //    else if (property.Key == typeof(Vector3))
            //    {
            //        p = new DclComponentProperty<Vector3>(property.Value, Vector3.zero);
            //    }
            //    else if (property.Key == typeof(Quaternion))
            //    {
            //        p = new DclComponentProperty<Quaternion>(property.Value, Quaternion.identity);
            //    }
            //    else
            //    {
            //        throw new ArgumentOutOfRangeException();
            //    }
            //
            //    Properties.Add(p);
            //}
        }
    }
}



