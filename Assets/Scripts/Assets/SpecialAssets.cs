using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Assets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

#pragma warning disable CS0649

public class SpecialAssets : MonoBehaviour
{
    public class GameObjectProvider : CommonAssetTypes.IModelProvider
    {
        private readonly Stack<CommonAssetTypes.GameObjectPoolEntry> pool = new();
        private readonly GameObject template;
        private readonly Transform poolParent;

        public GameObjectProvider(GameObject template, Transform poolParent)
        {
            this.template = template;
            this.poolParent = poolParent;
        }

        public void ReturnToPool(CommonAssetTypes.GameObjectInstance modelInstance)
        {
            foreach (var onReturnToPool in modelInstance.gameObject.GetComponentsInChildren<IOnReturnToPool>())
            {
                onReturnToPool.OnReturnToPool();
            }

            modelInstance.gameObject.transform.SetParent(poolParent, false);
            modelInstance.gameObject.SetActive(false);

            Assert.IsFalse(pool.Any(e => e.modelInstance == modelInstance));

            pool.Push(new CommonAssetTypes.GameObjectPoolEntry(modelInstance));
        }

        public CommonAssetTypes.GameObjectInstance CreateInstance()
        {
            if (pool.Count > 0)
            {
                var modelInstance = pool.Pop().modelInstance;
                modelInstance.gameObject.SetActive(true);
                return modelInstance;
            }

            // else
            return new CommonAssetTypes.GameObjectInstance(Object.Instantiate(template), this);
        }
    }


    [SerializeField]
    private GameObject errorTemplate;

    [SerializeField]
    private GameObject loadingTemplate;

    [SerializeField]
    private GameObject assetFolderUiElementTemplate;

    [SerializeField]
    private GameObject assetButtonUiElementTemplate;


    public GameObjectProvider errorModelProvider;
    public GameObjectProvider loadingModelProvider;

    public GameObjectProvider assetFolderUiElement;
    public GameObjectProvider assetButtonUiElement;

    public Transform poolParent;

    [Inject]
    public void Construct()
    {
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.IsNotNull(errorTemplate);
        Assert.IsNotNull(loadingTemplate);
        Assert.IsNotNull(assetFolderUiElementTemplate);
        Assert.IsNotNull(assetButtonUiElementTemplate);

        errorModelProvider = new GameObjectProvider(errorTemplate, poolParent);
        loadingModelProvider = new GameObjectProvider(loadingTemplate, poolParent);
        assetFolderUiElement = new GameObjectProvider(assetFolderUiElementTemplate, poolParent);
        assetButtonUiElement = new GameObjectProvider(assetButtonUiElementTemplate, poolParent);
        // ReSharper restore ExpressionIsAlwaysNull
    }
}

public interface IOnReturnToPool
{
    void OnReturnToPool();
}
