using System;
using JetBrains.Annotations;

public abstract class ValueStrategy<T>
{
    protected T value;

    public T currentValue => value;

    [CanBeNull]
    private Action<T> applyValueInternal;

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

    public static implicit operator ValueStrategy<T>(T value)
    {
        return new SetValueOnceStrategy<T>(value);
    }

    protected ValueStrategy(T initialValue)
    {
        value = initialValue;
    }
}

public class SetValueOnceStrategy<T> : ValueStrategy<T>
{
    public SetValueOnceStrategy(T value) : base(value)
    {
    }

    protected override void TriggerApplyValue()
    {
        applyValue?.Invoke(value);
    }
}

public class SetValueByFunction<T> : ValueStrategy<T>
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
