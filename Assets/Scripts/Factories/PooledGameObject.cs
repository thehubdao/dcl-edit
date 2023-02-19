using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Factories
{
    public class PooledGameObject<TComponent> : MonoBehaviour where TComponent : PooledGameObject<TComponent>
    {
        //Globals
        private GameObjectPoolFactory<TComponent> ownPoolInternal = null;

        public GameObjectPoolFactory<TComponent> ownPool
        {
            get => ownPoolInternal;
            set
            {
                if (ownPoolInternal != null)
                {
                    throw new System.Exception("Pool Cannot be changed.");
                }

                ownPoolInternal = value;
            }
        }

        public bool isInPool { get; private set; } = false;


        public GameObject InstantiateFromPool()
        {
            ResetGameObject();

            //reset transform
            transform.parent = null;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            isInPool = false;

            return gameObject;
        }

        public void DestroyToPool()
        {
            ownPool.OnGameObjectReturnToPool(this, () => isInPool = false);
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

        protected virtual void ResetGameObject()
        {
        }

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
}
