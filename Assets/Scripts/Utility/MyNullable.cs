using System;

namespace Assets.Scripts.Utility
{
    /**
     * A nullable type that is not restricted to just unmanaged types, but can handle all types
     */
    public class MyNullable<T>
    {
        public MyNullable()
        {
            IsNull = true;
        }

        public MyNullable(T value)
        {
            Value = value;
        }

        private T _value;

        public T Value
        {
            get
            {
                if (IsNull)
                    throw new NullReferenceException("Trying to access a null value in MyNullable");
                return _value;
            }
            set
            {
                if (value == null)
                {
                    _value = default;
                    IsNull = true;
                }
                else
                {
                    _value = value;
                    IsNull = false;
                }
            }
        }

        public bool IsNull { get; private set; }

        public bool TryGetValue(out T value)
        {
            if (IsNull)
            {
                value = default;
                return false;
            }

            value = _value;
            return true;
        }

        public static implicit operator MyNullable<T>(T value)
        {
            return new MyNullable<T>(value);
        }
    }
}