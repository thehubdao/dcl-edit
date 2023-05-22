using System;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;

public abstract class ValueBindStrategy<T>
{
    public abstract void SetValueToInput(T inputValue);
}

public class BindNameStrategy : ValueBindStrategy<string>
{
    private readonly Action<string> setValue;

    public BindNameStrategy(Action<string> setValue)
    {
        this.setValue = setValue;
    }

    public override void SetValueToInput(string inputValue)
    {
        setValue(inputValue);
    }
}

public class JustSetStrategy<T> : ValueBindStrategy<T>
{
    public override void SetValueToInput(T inputValue)
    {
    }
}
