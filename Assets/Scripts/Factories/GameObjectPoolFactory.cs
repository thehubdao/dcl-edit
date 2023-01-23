using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Factories
{
    public class GameObjectPoolFactory<TComponent> : PlaceholderFactory<TComponent> where TComponent : PooledGameObject<TComponent>
    {
        //Globals
        private readonly Stack<PooledGameObject<TComponent>> availableObjects = new Stack<PooledGameObject<TComponent>>();
        private readonly List<PooledGameObject<TComponent>> allObjects = new List<PooledGameObject<TComponent>>();
        public Transform poolParent { get; private set; }


        [Inject]
        void Construct()
        {
            poolParent = new GameObject($"GameObjectPool - {nameof(TComponent)}").transform;
            poolParent.gameObject.SetActive(false);
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

            return (TComponent) instance;
        }

        public void ReturnAllObjectsToPool()
        {
            foreach (PooledGameObject<TComponent> poolsObjects in allObjects)
            {
                if (!poolsObjects.isInPool)
                {
                    poolsObjects.DestroyToPool();
                }
            }
        }

        public void OnGameObjectReturnToPool(PooledGameObject<TComponent> instance, Action isInPoolSetter)
        {
            instance.transform.parent = poolParent;
            isInPoolSetter();
            availableObjects.Push(instance);
        }

        private PooledGameObject<TComponent> IncreaseCapacity()
        {
            PooledGameObject<TComponent> instance = base.Create();

            instance.ownPool = this;
            allObjects.Add(instance);

            return instance;
        }
    }
}
