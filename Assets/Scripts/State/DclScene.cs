using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.State
{
    public class DclScene : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _entityRootObject;

        public DclEntity[] AllEntities => _entityRootObject.GetComponentsInChildren<DclEntity>();

        public IEnumerable<DclEntity> EntitiesInSceneRoot => AllEntities.Where(e => e.Parent==null);

        
    }
}
