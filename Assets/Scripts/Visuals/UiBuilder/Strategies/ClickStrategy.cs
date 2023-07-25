using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ClickStrategy
{
    public struct EventData
    {
        [NotNull]
        public GameObject gameObject;

        [NotNull]
        public Vector2 position;
    }

    public ClickStrategy([CanBeNull] LeftClickStrategy leftClickStrategy = null, [CanBeNull] RightClickStrategy rightClickStrategy = null)
    {
        this.leftClickStrategy = leftClickStrategy;
        this.rightClickStrategy = rightClickStrategy;
    }

    [CanBeNull]
    public LeftClickStrategy leftClickStrategy;

    [CanBeNull]
    public RightClickStrategy rightClickStrategy;
}

public class LeftClickStrategy
{
    [NotNull]
    public Action<ClickStrategy.EventData> onLeftClick;

    public LeftClickStrategy([NotNull] Action<ClickStrategy.EventData> onLeftClick)
    {
        this.onLeftClick = onLeftClick;
    }

    // implicit conversion to ClickStrategy
    public static implicit operator ClickStrategy(LeftClickStrategy leftClickStrategy)
    {
        return new ClickStrategy(leftClickStrategy: leftClickStrategy);
    }
}

public class RightClickStrategy
{
    [NotNull]
    public Action<ClickStrategy.EventData> onRightClick;

    public RightClickStrategy([NotNull] Action<ClickStrategy.EventData> onRightClick)
    {
        this.onRightClick = onRightClick;
    }

    // implicit conversion to ClickStrategy
    public static implicit operator ClickStrategy(RightClickStrategy rightClickStrategy)
    {
        return new ClickStrategy(rightClickStrategy: rightClickStrategy);
    }
}
