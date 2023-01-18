using System.Collections;
using UnityEditor;
using UnityEngine;

public class PooledGameObject<TComponent> : MonoBehaviour where TComponent : PooledGameObject<TComponent>
{
    //Globals
    private GameObjectPoolFactory<TComponent> ownPool = null;
    public GameObjectPoolFactory<TComponent> OwnPool { get { return ownPool; } set { if (ownPool != null) { throw new System.Exception("Pool Cannot be changed."); } ownPool = value; } }
    public bool IsInPool { get; private set; } = false;


    public GameObject InstantiateFromPool()
    {
        ResetGameObject();

        //reset transform
        transform.parent = null;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        
        IsInPool = false;

        return gameObject;
    }

    public void DestroyToPool()
    {
        OwnPool.OnGameObjectReturnToPool(this, () => IsInPool = false);
    }

    public void DestroyToPool(float t)
    {
        StartCoroutine(DestroyToPoolDelayed(t));
    }

    private IEnumerator DestroyToPoolDelayed(float t)
    {
        yield return new WaitForSeconds(t);
        DestroyToPool();
    }

    protected virtual void ResetGameObject() { }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("Pooled GameObject Destroyed", this);
        }
    }
#endif
}
