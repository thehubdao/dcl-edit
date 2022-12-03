using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Visuals
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

        public struct PooledObject
        {
            public GameObject gameObject;
            public AtomType atomType;
        }

        private Dictionary<AtomType, List<GameObject>> atomPool = new Dictionary<AtomType, List<GameObject>>();

        private PooledObject GetAtomObjectFromPool(AtomType type)
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

                    return new PooledObject {atomType = type, gameObject = go};
                }
            }

            return new PooledObject {atomType = type, gameObject = InstantiateObject(type)};
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

        private void ReturnAtomsToPool([CanBeNull] PooledObject objects)
        {
            Stats.returnToPoolCount++;

            objects.gameObject.SetActive(false);
            objects.gameObject.transform.SetParent(GameObject.Find("SceneContext").transform);

            if (!atomPool.ContainsKey(objects.atomType))
            {
                atomPool[objects.atomType] = new List<GameObject>();
            }

            atomPool[objects.atomType].Add(objects.gameObject);
        }

        #endregion // Object Pool

        public class AtomGameObject
        {
            public PooledObject gameObject;
            public int height;
            public int position;
        }

        public abstract class Atom
        {
            public NewUiBuilder uiBuilder;
            public AtomGameObject gameObject;

            protected Atom(NewUiBuilder uiBuilder)
            {
                this.uiBuilder = uiBuilder;
            }

            public abstract bool Update([NotNull] Data newData, int newPosition);

            public virtual void Remove()
            {
                uiBuilder.ReturnAtomsToPool(gameObject.gameObject);
            }

            public abstract class Data
            {
            }

            protected void UpdatePositionAndSize(int position, int height)
            {
                // Update position and size
                var tf = gameObject.gameObject.gameObject.GetComponent<RectTransform>();

                tf.offsetMin = Vector2.zero;
                tf.offsetMax = Vector2.zero;

                tf.anchoredPosition = new Vector3(0, -position, 0);
                tf.sizeDelta = new Vector2(tf.sizeDelta.x, height);

                gameObject.position = position;
                gameObject.height = height;
            }
        }

        public class PanelAtom : Atom
        {
            public new class Data : Atom.Data
            {
                public List<Atom.Data> childDates;
            }

            public Data data;

            public List<Atom> childAtoms = new List<Atom>();

            private GameObject content = null;

            public override bool Update(Atom.Data newData, int newPosition)
            {
                Stats.atomsUpdatedCount++;

                var hasChanged = false;
                var newPanelData = (Data) newData;

                if (gameObject == null)
                {
                    MakeNewAtomGameObject();

                    hasChanged = true;
                }

                var newHeight = -1;

                if (!newPanelData.Equals(data))
                {
                    UpdateData(newPanelData, out var changed, out newHeight);

                    hasChanged = hasChanged || changed;
                }

                if (newPosition != gameObject.position || (newHeight > 0 && newHeight != gameObject.height))
                {
                    UpdatePositionAndSize(newPosition, newHeight);
                    hasChanged = true;
                }

                return hasChanged;
            }

            public override void Remove()
            {
                foreach (var childAtom in childAtoms)
                {
                    childAtom.Remove();
                }

                base.Remove();
            }

            protected virtual void MakeNewAtomGameObject()
            {
                // Make new AtomGameObject
                gameObject = new AtomGameObject()
                {
                    gameObject = uiBuilder.GetAtomObjectFromPool(AtomType.Panel),
                    height = 40,
                    position = -1
                };
            }

            protected void UpdateData(Data newPanelData, out bool hasChanged, out int endHeight)
            {
                hasChanged = false;

                // Update contents // TODO: Check, if an atom needs rebuilding
                foreach (var childAtom in childAtoms)
                {
                    childAtom.Remove();
                }

                childAtoms.Clear();

                var lastPos = 0;
                foreach (var childData in newPanelData.childDates)
                {
                    var childAtom = MakeChildAtom(childData);

                    var changed = childAtom.Update(childData, lastPos);

                    if (changed)
                    {
                        hasChanged = true;
                    }

                    childAtom.gameObject.gameObject.gameObject.transform.SetParent(
                        gameObject.gameObject.gameObject.GetComponent<PanelHandler>().Content.transform, false);

                    lastPos += childAtom.gameObject.height;

                    childAtoms.Add(childAtom);
                }

                endHeight = lastPos;
            }

            private Atom MakeChildAtom(Atom.Data childData)
            {
                return childData switch
                {
                    TextAtom.Data _ => new TextAtom(uiBuilder),
                    PanelAtom.Data _ => new PanelAtom(uiBuilder),
                    _ => throw new ArgumentException()
                };
            }

            public PanelAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
            {
            }
        }

        public class TextAtom : Atom
        {
            public new class Data : Atom.Data
            {
                public string text;

                public bool Equals(Data other)
                {
                    if (other == null)
                    {
                        return false;
                    }

                    return text == other.text;
                }
            }

            private Data data;

            public override bool Update(Atom.Data newData, int newPosition)
            {
                Stats.atomsUpdatedCount++;

                var hasChanged = false;
                var newTextData = (Data) newData;

                if (gameObject == null)
                {
                    // Make new game object
                    gameObject = new AtomGameObject
                    {
                        gameObject = uiBuilder.GetAtomObjectFromPool(AtomType.Text),
                        height = 50,
                        position = -1
                    };

                    hasChanged = true;
                }

                if (!newTextData.Equals(data))
                {
                    // Update data
                    var textHandler = gameObject.gameObject.gameObject.GetComponent<TextHandler>();
                    textHandler.text = newTextData.text;
                    data = newTextData;
                }

                if (newPosition != gameObject.position /* or height has changed */)
                {
                    // Update position and size
                    var tf = gameObject.gameObject.gameObject.GetComponent<RectTransform>();

                    tf.offsetMin = Vector2.zero;
                    tf.offsetMax = Vector2.zero;

                    tf.anchoredPosition = new Vector3(0, -newPosition, 0);
                    tf.sizeDelta = new Vector2(tf.sizeDelta.x, gameObject.height);

                    hasChanged = true;
                }

                return hasChanged;
            }


            public TextAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
            {
            }
        }


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
