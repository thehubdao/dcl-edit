using System;

public class SetValueStrategy<T>
{
    public Func<T> value { get; }

    public static implicit operator SetValueStrategy<T>(T value)
    {
        return new SetValueStrategy<T>(() => value);
    }

    public SetValueStrategy(Func<T> value)
    {
        this.value = value;
    }
}
