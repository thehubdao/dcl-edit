using System;
using JetBrains.Annotations;

public abstract class SetValueStrategy<T>
{
    protected T value;

    public T currentValue => value;

    [CanBeNull]
    private Action<T> applyValueInternal;


    /// <summary>
    /// a callback that will be called when a new value is present
    /// </summary>
    /// <remarks>IMPORTANT: Make sure, that you remove your callback from the previous SetValueStrategy</remarks>
    [CanBeNull]
    public Action<T> applyValue
    {
        set
        {
            applyValueInternal = value;
            TriggerApplyValue();
        }
        protected get => applyValueInternal;
    }

    protected abstract void TriggerApplyValue();

    public static implicit operator SetValueStrategy<T>(T value)
    {
        return new SetValueOnceStrategy<T>(value);
    }

    protected SetValueStrategy(T initialValue)
    {
        value = initialValue;
    }
}

public class SetValueOnceStrategy<T> : SetValueStrategy<T>
{
    public SetValueOnceStrategy(T value) : base(value)
    {
    }

    protected override void TriggerApplyValue()
    {
        applyValue?.Invoke(value);
    }
}

public class SetValueByFunction<T> : SetValueStrategy<T>
{
    public SetValueByFunction(T initialValue) : base(initialValue)
    {
    }

    public void SetValue(T newValue)
    {
        value = newValue;
        TriggerApplyValue();
    }

    protected override void TriggerApplyValue()
    {
        applyValue?.Invoke(value);
    }
}
