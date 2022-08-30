using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.SceneState
{
    public class DclComponent
    {
        #region Property
        public abstract class DclComponentProperty
        {
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

            public DclComponentProperty<T> GetConcrete<T>()
            {
                if (!(this is DclComponentProperty<T> dclComponentProperty))
                {
                    throw new System.Exception("Property is not of type " + typeof(T).Name);
                }
                return dclComponentProperty;
            }
        }

        public class DclComponentProperty<T> : DclComponentProperty
        {
            private T _fixedValue;
            private T _floatingValue;

            private bool _isFloating;

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
             * <summary>Is the value currently floating</summary>>
             */
            public bool IsFloating => _isFloating;

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
             * <summary>Reset the floating value</summary>
             */
            public void ResetFloating()
            {
                _isFloating = false;
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



