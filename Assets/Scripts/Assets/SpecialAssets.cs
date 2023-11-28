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
    public class SpecialModelProvider : CommonAssetTypes.IModelProvider
    {
        private readonly Stack<CommonAssetTypes.ModelPoolEntry> pool = new();
        private readonly GameObject template;

        public SpecialModelProvider(GameObject template)
        {
            this.template = template;
        }

        public void ReturnToPool(CommonAssetTypes.ModelInstance modelInstance)
        {
            modelInstance.gameObject.SetActive(false);
            pool.Push(new CommonAssetTypes.ModelPoolEntry(modelInstance));
        }

        public CommonAssetTypes.ModelInstance CreateInstance()
        {
            if (pool.Count > 0)
            {
                var modelInstance = pool.Pop().modelInstance;
                modelInstance.gameObject.SetActive(true);
                return modelInstance;
            }

            // else
            return new CommonAssetTypes.ModelInstance(Object.Instantiate(template), this);
        }
    }


    [SerializeField]
    private GameObject errorTemplate;

    [SerializeField]
    private GameObject loadingTemplate;


    public SpecialModelProvider errorModelProvider;
    public SpecialModelProvider loadingModelProvider;

    [Inject]
    public void Construct()
    {
        // ReSharper disable ExpressionIsAlwaysNull
        Assert.IsNotNull(errorTemplate);
        Assert.IsNotNull(loadingTemplate);

        errorModelProvider = new SpecialModelProvider(errorTemplate);
        loadingModelProvider = new SpecialModelProvider(loadingTemplate);
        // ReSharper restore ExpressionIsAlwaysNull
    }
}
