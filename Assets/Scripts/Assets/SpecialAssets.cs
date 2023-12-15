using System;
using System.Collections;
using System.Collections.Generic;
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

        public GameObjectProvider(GameObject template)
        {
            this.template = template;
        }

        public void ReturnToPool(CommonAssetTypes.GameObjectInstance modelInstance)
        {
            modelInstance.gameObject.SetActive(false);
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

    [Inject]
    public void Construct()
    {
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.IsNotNull(errorTemplate);
        Assert.IsNotNull(loadingTemplate);
        Assert.IsNotNull(assetFolderUiElementTemplate);
        Assert.IsNotNull(assetButtonUiElementTemplate);

        errorModelProvider = new GameObjectProvider(errorTemplate);
        loadingModelProvider = new GameObjectProvider(loadingTemplate);
        assetFolderUiElement = new GameObjectProvider(assetFolderUiElementTemplate);
        assetButtonUiElement = new GameObjectProvider(assetButtonUiElementTemplate);
        // ReSharper restore ExpressionIsAlwaysNull
    }
}
