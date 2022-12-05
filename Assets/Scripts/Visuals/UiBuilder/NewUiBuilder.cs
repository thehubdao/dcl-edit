using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class NewUiBuilder
    {
        #region Statistics

        public static class Stats
        {
            public static int instantiateCount = 0;
            public static int destroyCount = 0;
            public static int getFromPoolCount = 0;
            public static int returnToPoolCount = 0;
            public static int atomsUpdatedCount = 0;

            public static void Dump()
            {
                Debug.Log($"Ui Builder stats: Instantiate count: {instantiateCount}, Destroy count: {destroyCount}, Get from Pool count: {getFromPoolCount}, Return to Pool count: {returnToPoolCount}, Atoms Updated count: {atomsUpdatedCount}");
            }
        }

        #endregion

        public enum AtomType
        {
            Title,
            Text,
            Spacer,
            Panel,
            PanelHeader,
            HierarchyItem,
            StringPropertyInput,
            NumberPropertyInput,
            BooleanPropertyInput,
            Vector3PropertyInput,
            ContextMenu,
            ContextMenuItem,
            ContextSubmenuItem,
            ContextMenuSpacerItem
        }


        // Dependencies
        private UnityState unityState;

        [Inject]
        private void Constructor(UnityState unityState)
        {
            this.unityState = unityState;
        }

        #region Object Pool


        private Dictionary<AtomType, List<GameObject>> atomPool = new Dictionary<AtomType, List<GameObject>>();

        public AtomGameObject GetAtomObjectFromPool(AtomType type)
        {
            // check for available object
            if (atomPool.TryGetValue(type, out var objectList))
            {
                if (objectList.Count > 0)
                {
                    Stats.getFromPoolCount++;

                    var go = objectList[objectList.Count - 1];
                    objectList.RemoveAt(objectList.Count - 1);

                    go.SetActive(true);

                    return new AtomGameObject {atomType = type, gameObject = go};
                }
            }

            return new AtomGameObject {atomType = type, gameObject = InstantiateObject(type)};
        }

        private GameObject InstantiateObject(AtomType type)
        {
            Stats.instantiateCount++;

            return type switch
            {
                AtomType.Title => Object.Instantiate(unityState.TitleAtom),
                AtomType.Text => Object.Instantiate(unityState.TextAtom),
                AtomType.Panel => Object.Instantiate(unityState.PanelAtom),
                AtomType.PanelHeader => Object.Instantiate(unityState.PanelHeaderAtom),
                AtomType.HierarchyItem => Object.Instantiate(unityState.HierarchyItemAtom),
                AtomType.StringPropertyInput => Object.Instantiate(unityState.StringInputAtom),
                AtomType.NumberPropertyInput => Object.Instantiate(unityState.NumberInputAtom),
                AtomType.BooleanPropertyInput => Object.Instantiate(unityState.BooleanInputAtom),
                AtomType.Vector3PropertyInput => Object.Instantiate(unityState.Vector3InputAtom),
                AtomType.ContextMenu => Object.Instantiate(unityState.ContextMenuAtom),
                AtomType.ContextMenuItem => Object.Instantiate(unityState.ContextMenuItemAtom),
                AtomType.ContextSubmenuItem => Object.Instantiate(unityState.ContextSubmenuItemAtom),
                AtomType.ContextMenuSpacerItem => Object.Instantiate(unityState.ContextMenuSpacerItemAtom),
                _ => null
            };
        }

        public void ReturnAtomsToPool([CanBeNull] AtomGameObject atomGameObject)
        {
            if (atomGameObject == null)
                return;

            Stats.returnToPoolCount++;

            atomGameObject.gameObject.SetActive(false);
            atomGameObject.gameObject.transform.SetParent(GameObject.Find("SceneContext").transform);

            if (!atomPool.ContainsKey(atomGameObject.atomType))
            {
                atomPool[atomGameObject.atomType] = new List<GameObject>();
            }

            atomPool[atomGameObject.atomType].Add(atomGameObject.gameObject);
        }

        #endregion // Object Pool


        // ----------------------------

        private PanelAtom currentRootAtom = null;
        private GameObject parentObject;

        public NewUiBuilder(GameObject parent)
        {
            parentObject = parent;
        }


        public void Update(PanelAtom.Data newData)
        {
            // Create new root atom if not exists
            currentRootAtom ??= new PanelAtom(this);

            currentRootAtom.Update(newData, 0);
            currentRootAtom.gameObject.gameObject.gameObject.transform.SetParent(parentObject.transform);
        }

        public class Factory : PlaceholderFactory<GameObject, NewUiBuilder>
        {
        }
    }
}
