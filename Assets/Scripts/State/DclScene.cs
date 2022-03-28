using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exception = System.Exception;

namespace Assets.Scripts.State
{
    public class DclScene : MonoBehaviour
    {
        [SerializeField]
        private GameObject _entityRootObject;

        public GameObject EntityRootObject => _entityRootObject;

        public DclEntity[] AllEntities => _entityRootObject.GetComponentsInChildren<DclEntity>();

        public IEnumerable<DclEntity> EntitiesInSceneRoot => AllEntities.Where(e => e.Parent == null);

        public DclEntity GetEntityFormId(Guid id)
        {
            try
            {
                return AllEntities.First(e => e.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
