using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private readonly List<Action> doOnNextUpdate = new();
    private readonly List<Action> doOnNextLateUpdate = new();

    void Update()
    {
        foreach (var action in doOnNextUpdate)
        {
            action();
        }

        doOnNextUpdate.Clear();
    }

    void LateUpdate()
    {
        foreach (var action in doOnNextLateUpdate)
        {
            action();
        }

        doOnNextLateUpdate.Clear();
    }

    public void DoOnNextUpdate(Action action)
    {
        doOnNextUpdate.Add(action);
    }

    public void DoOnNextLateUpdate(Action action)
    {
        doOnNextLateUpdate.Add(action);
    }
}
