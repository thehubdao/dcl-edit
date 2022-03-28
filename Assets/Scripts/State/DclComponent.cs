using ICSharpCode.NRefactory.Ast;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.State
{
    public class DclComponent : MonoBehaviour
    {
        #region Property
        public abstract class DclComponentProperty
        {
            public enum PropertyType
            {
                None,
                String,
                Int,
                Float,
                Vector3,
            }

            public PropertyType Type =>
                this switch
                {
                    DclComponentProperty<string> _ => PropertyType.String,
                    DclComponentProperty<int> _ => PropertyType.Int,
                    DclComponentProperty<float> _ => PropertyType.Float,
                    DclComponentProperty<Vector3> _ => PropertyType.Vector3,
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
             * <param name="initialValue">The initial fixed value</param>
             */
            public DclComponentProperty(T initialValue)
            {
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
        
        
        private string _name;

        private DclComponentProperty[] _properties;

        
    }
}

