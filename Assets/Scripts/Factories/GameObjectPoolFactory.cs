using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class GameObjectPoolFactory<TComponent> : PlaceholderFactory<TComponent> where TComponent : PooledGameObject<TComponent>
{
    //Globals
    private Stack<PooledGameObject<TComponent>> availableObjects = new Stack<PooledGameObject<TComponent>>();
    private List<PooledGameObject<TComponent>> allObjects = new List<PooledGameObject<TComponent>>();
    public Transform PoolParent { get; private set; }


    [Inject]
    void Construct()
    {
        PoolParent = new GameObject($"GameObjectPool - {nameof(TComponent)}").transform;
        PoolParent.gameObject.SetActive(false);
    }

    [NotNull]
    public override TComponent Create()
    {
        PooledGameObject<TComponent> instance;

        // get object from pool or instantiate new one
        if (availableObjects.Count > 0)
        {
            instance = availableObjects.Pop();
            instance.InstantiateFromPool();
        }
        else
        {
            instance = IncreaseCapacity();
        }

        return (TComponent)instance;
    }

    public void ReturnAllObjectsToPool()
    {
        foreach (PooledGameObject<TComponent> poolsObjects in allObjects)
        {
            if (!poolsObjects.IsInPool)
            {
                poolsObjects.DestroyToPool();
            }
        }
    }

    public void OnGameObjectReturnToPool(PooledGameObject<TComponent> instance, Action isInPoolSetter)
    {
        instance.transform.parent = PoolParent;
        isInPoolSetter();
        availableObjects.Push(instance);
    }

    private PooledGameObject<TComponent> IncreaseCapacity()
    {
        PooledGameObject<TComponent> instance = base.Create();

        instance.OwnPool = this;
        allObjects.Add(instance);

        return instance;
    }
}
