using System;
using JetBrains.Annotations;

public class ValueBindStrategy<T>
{
    public ValueBindStrategy(
        [NotNull] Func<T> value,
        [CanBeNull] Action<T> onValueSubmitted = null,
        [CanBeNull] Action<string[]> onErrorSubmitted = null,
        [CanBeNull] Action<T> onValueChanged = null,
        [CanBeNull] Action<string[]> onErrorChanged = null)
    {
        this.value = value;
        this.onValueSubmitted = onValueSubmitted;
        this.onErrorSubmitted = onErrorSubmitted;
        this.onValueChanged = onValueChanged;
        this.onErrorChanged = onErrorChanged;
    }

    [NotNull]
    public Func<T> value;

    [CanBeNull]
    public Action<T> onValueSubmitted;

    [CanBeNull]
    public Action<string[]> onErrorSubmitted;

    [CanBeNull]
    public Action<T> onValueChanged;

    [CanBeNull]
    public Action<string[]> onErrorChanged;
}
