using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;

public class AssetFormatLoadedModel : CommonAssetTypes.AssetFormat, CommonAssetTypes.IModelProvider
{
    public override string formatName => "Loaded model";
    public override string hash { get; }

    public override CommonAssetTypes.Availability availability => availabilityInternal;

    private CommonAssetTypes.Availability availabilityInternal;

    private GameObject modelTemplate;

    private Stack<CommonAssetTypes.ModelPoolEntry> modelPool = new();

    public AssetFormatLoadedModel(string hash)
    {
        this.hash = hash;
        availabilityInternal = CommonAssetTypes.Availability.Loading;
    }

    public void SetModel(GameObject modelTemplate)
    {
        this.modelTemplate = modelTemplate;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }

    public void SetError()
    {
        availabilityInternal = CommonAssetTypes.Availability.Error;
    }

    public void ReturnToPool(CommonAssetTypes.ModelInstance modelInstance)
    {
        modelInstance.gameObject.SetActive(false);
        modelPool.Push(new CommonAssetTypes.ModelPoolEntry(modelInstance));
    }

    public CommonAssetTypes.ModelInstance CreateInstance()
    {
        if (modelPool.Count > 0)
        {
            var modelInstance = modelPool.Pop().modelInstance;
            modelInstance.gameObject.SetActive(true);
            return modelInstance;
        }

        // else
        return new CommonAssetTypes.ModelInstance(Object.Instantiate(modelTemplate), this);
    }
}
