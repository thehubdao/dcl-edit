using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class ClickStrategy
{
    public struct EventData
    {
        [NotNull]
        public GameObject gameObject;

        [NotNull]
        public Vector2 position;
    }
}

public class LeftClickStrategy : ClickStrategy
{
    [NotNull]
    public Action<EventData> onLeftClick;
}

public class RightClickStrategy : ClickStrategy
{
    [NotNull]
    public Action<EventData> onRightClick;
}
