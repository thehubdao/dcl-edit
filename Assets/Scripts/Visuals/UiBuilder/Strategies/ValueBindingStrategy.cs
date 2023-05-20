using System;
using JetBrains.Annotations;

public abstract class ValueStrategy<T>
{
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


    protected ValueStrategy()
    {
    }
}

public class SetValueOnceStrategy<T> : ValueStrategy<T>
{
    private readonly T value;

    protected SetValueOnceStrategy(T value) : base()
    {
        this.value = value;
    }

    protected override void TriggerApplyValue()
    {
        applyValue?.Invoke(value);
    }
}
