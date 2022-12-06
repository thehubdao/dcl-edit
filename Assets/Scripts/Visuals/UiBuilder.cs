using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.PropertyHandler;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Visuals
{
    public class UiBuilder
    {
        private enum AtomType
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

        private struct UiAtom
        {
            public AtomType Type;
            public Func<MakeGmReturn> MakeGameObject;
        }

        private struct MakeGmReturn
        {
            public GameObject go;
            public int height;
        }

        public struct UiPropertyActions<T>
        {
            public Action<T> OnChange;
            public Action<T> OnSubmit;
            public Action<T> OnAbort;
        }

        private readonly List<UiAtom> _atoms = new List<UiAtom>();

        // Dependencies
        UnityState _unityState;

        [Inject]
        private void Constructor(UnityState unityState)
        {
            _unityState = unityState;
        }

        public UiBuilder Title(string title)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Title,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.Title);

                    var text = go.GetComponent<TextMeshProUGUI>();

                    text.text = title;

                    return new MakeGmReturn { go = go, height = 130 };
                }
            });

            return this;
        }

        public UiBuilder Text(string text)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Title,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.Text);

                    var tmpText = go.GetComponent<TextMeshProUGUI>();

                    tmpText.text = text;

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder Spacer(int height)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Spacer,
                MakeGameObject = () =>
                {
                    var gameObject = new GameObject("Spacer");
                    gameObject.AddComponent<RectTransform>();
                    return new MakeGmReturn { go = gameObject, height = height };
                }
            });

            return this;
        }

        public UiBuilder Panel(UiBuilder content)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Panel,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.Panel);

                    var handler = go.GetComponent<PanelHandler>();

                    content.ClearAndMakePanel(handler.Content);

                    return new MakeGmReturn { go = go, height = content.CurrentHeight + 40 };
                }
            });

            return this;
        }

        /**
         * Creates a panel header with a title and a optional close button.
         * @param title The title of the panel.
         * @param onClose The function to be called, when the close button is pressed. When this is null, the close button will be hidden
         */
        public UiBuilder PanelHeader(string title, [CanBeNull] UnityAction onClose = null)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.PanelHeader,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.PanelHeader);

                    var handler = go.GetComponent<PanelHeaderHandler>();
                    handler.Title.text = title;

                    if (onClose != null)
                    {
                        handler.CloseButton.onClick.AddListener(onClose);
                    }
                    else
                    {
                        handler.CloseButton.gameObject.SetActive(false);
                    }

                    return new MakeGmReturn { go = go, height = 60 };
                }
            });

            return this;
        }

        public UiBuilder HierarchyItem(string name, int level, bool hasChildren, bool isExpanded, TextHandler.TextStyle textStyle, HierarchyItemHandler.UiHierarchyItemActions actions)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.HierarchyItem,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.HierarchyItem);

                    var hierarchyItem = go.GetComponent<HierarchyItemHandler>();

                    hierarchyItem.text.text = name;
                    hierarchyItem.indent.offsetMin = new Vector2(20 * level, 0);
                    hierarchyItem.text.textStyle = textStyle;
                    hierarchyItem.actions = actions;

                    if (hasChildren)
                    {
                        hierarchyItem.showArrow = true;
                        hierarchyItem.isExpanded = isExpanded;
                    }
                    else
                    {
                        hierarchyItem.showArrow = false;
                    }

                    return new MakeGmReturn { go = go, height = 30 };
                }
            });

            return this;
        }

        public UiBuilder StringPropertyInput(string name, string placeholder, string currentContents, UiPropertyActions<string> actions)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.StringPropertyInput,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.StringPropertyInput);

                    var stringProperty = go.GetComponent<StringPropertyHandler>();

                    stringProperty.propertyNameText.text = name;
                    stringProperty.stringInput.SetCurrentText(currentContents);
                    stringProperty.stringInput.SetPlaceHolder(placeholder);

                    // setup actions
                    stringProperty.SetActions(actions);

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder NumberPropertyInput(string name, string placeholder, float currentContents, UiPropertyActions<float> actions)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.NumberPropertyInput,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.NumberPropertyInput);

                    var numberProperty = go.GetComponent<NumberPropertyHandler>();

                    numberProperty.propertyNameText.text = name;
                    numberProperty.numberInput.SetCurrentNumber(currentContents);
                    numberProperty.numberInput.TextInputHandler.SetPlaceHolder(placeholder);

                    numberProperty.SetActions(actions);

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder BooleanPropertyInput(string name, bool currentContents, UiPropertyActions<bool> actions)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.BooleanPropertyInput,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.BooleanPropertyInput);

                    var booleanProperty = go.GetComponent<BooleanPropertyHandler>();

                    booleanProperty.PropertyNameText.text = name;
                    booleanProperty.CheckBoxInput.isOn = currentContents;

                    booleanProperty.SetActions(actions);

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder Vector3PropertyInput(string name, string[] placeholders, Vector3 currentContents, UiPropertyActions<Vector3> actions)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Vector3PropertyInput,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.Vector3PropertyInput);

                    var vector3PropertyHandler = go.GetComponent<Vector3PropertyHandler>();

                    vector3PropertyHandler.propertyNameText.text = name;

                    vector3PropertyHandler.numberInputX.SetCurrentNumber(currentContents.x);
                    vector3PropertyHandler.numberInputX.TextInputHandler.SetPlaceHolder(placeholders[0]);

                    vector3PropertyHandler.numberInputY.SetCurrentNumber(currentContents.y);
                    vector3PropertyHandler.numberInputY.TextInputHandler.SetPlaceHolder(placeholders[1]);

                    vector3PropertyHandler.numberInputZ.SetCurrentNumber(currentContents.z);
                    vector3PropertyHandler.numberInputZ.TextInputHandler.SetPlaceHolder(placeholders[2]);

                    vector3PropertyHandler.SetActions(actions);

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder ContextMenu(UiBuilder content)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.ContextMenu,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.ContextMenu);

                    var panelHandler = go.GetComponent<PanelHandler>();
                    content.ClearAndMake(panelHandler.Content);

                    // Use height required by content but not more than screen height
                    var height = Mathf.Min(Screen.height, content.CurrentHeight);
                    return new MakeGmReturn { go = go, height = height };
                }
            });

            return this;
        }

        public UiBuilder ContextMenuTextItem(Guid menuId, string title, UnityAction onClick, bool isDisabled, ContextMenuSystem contextMenuSystem)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.ContextMenuItem,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.ContextMenuItem);

                    var text = go.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = title;

                    var button = go.GetComponent<Button>();
                    button.onClick.AddListener(onClick);
                    button.onClick.AddListener(contextMenuSystem.CloseMenu);
                    if (isDisabled) button.interactable = false;

                    var hoverHandler = go.GetComponent<ContextMenuHoverHandler>();
                    hoverHandler.OnHoverAction = () => contextMenuSystem.CloseMenusUntil(menuId);

                    return new MakeGmReturn { go = go, height = 30 };
                }
            });

            return this;
        }

        public UiBuilder ContextSubmenuItem(Guid menuId, Guid submenuId, string title, List<ContextMenuItem> submenuItems, float menuWidth, ContextMenuSystem contextMenuSystem)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.ContextSubmenuItem,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.ContextSubmenuItem);
                    var rect = go.GetComponent<RectTransform>();

                    var text = go.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = title;

                    var hoverHandler = go.GetComponent<ContextMenuHoverHandler>();
                    hoverHandler.OnHoverAction = () =>
                    {
                        contextMenuSystem.CloseMenusUntil(menuId);

                        Vector3 rightExpandPosition = new Vector3(rect.position.x + menuWidth, rect.position.y, rect.position.z);
                        Vector3 leftExpandPosition = new Vector3(rect.position.x, rect.position.y, rect.position.z);
                        contextMenuSystem.OpenSubmenu(
                            submenuId,
                            new List<ContextMenuState.Placement>{
                                new ContextMenuState.Placement{position = rightExpandPosition, expandDirection = ContextMenuState.Placement.Direction.Right},
                                new ContextMenuState.Placement{position = leftExpandPosition, expandDirection = ContextMenuState.Placement.Direction.Left}
                            },
                            submenuItems
                        );
                    };

                    return new MakeGmReturn { go = go, height = 30 };
                }
            });

            return this;
        }

        public UiBuilder ContextMenuSpacerItem(Guid menuId, ContextMenuSystem contextMenuSystem)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.ContextMenuSpacerItem,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.ContextMenuSpacerItem);

                    var hoverHandler = go.GetComponent<ContextMenuHoverHandler>();
                    hoverHandler.OnHoverAction = () => contextMenuSystem.CloseMenusUntil(menuId);

                    return new MakeGmReturn { go = go, height = 15 };
                }
            });

            return this;
        }

        public void ClearAndMake(GameObject parent)
        {
            Clear(parent);
            Make(parent);
        }

        private void ClearAndMakePanel(GameObject parent)
        {
            Clear(parent);
            Make(parent, true);
        }

        private void Clear(GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                ReturnAtomsToPool(child.gameObject);
            }
        }

        public int CurrentHeight { get; private set; }

        private void Make(GameObject parent, bool isInPanel = false)
        {
            var heightCounter = 0;
            foreach (var atom in _atoms)
            {
                var madeGameObject = atom.MakeGameObject();

                var tf = madeGameObject.go.GetComponent<RectTransform>();

                tf.SetParent(parent.transform);

                tf.offsetMin = Vector2.zero;
                tf.offsetMax = Vector2.zero;

                tf.anchoredPosition = new Vector3(0, heightCounter, 0);

                tf.sizeDelta = new Vector2(tf.sizeDelta.x, madeGameObject.height);


                heightCounter -= madeGameObject.height;
            }

            var parentRectTransform =
                isInPanel ?
                    parent.GetComponentInParent<PanelHandler>().GetComponent<RectTransform>() :
                    parent.GetComponent<RectTransform>();

            CurrentHeight = -heightCounter;

            parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, CurrentHeight + (isInPanel ? 40 : 0));
        }


        // Util
        // TODO: Implement pool
        // TODO: make static
        private GameObject GetAtomObjectFromPool(AtomType type)
        {
            return type switch
            {
                AtomType.Title => Object.Instantiate(_unityState.TitleAtom),
                AtomType.Text => Object.Instantiate(_unityState.TextAtom),
                AtomType.Panel => Object.Instantiate(_unityState.PanelAtom),
                AtomType.PanelHeader => Object.Instantiate(_unityState.PanelHeaderAtom),
                AtomType.HierarchyItem => Object.Instantiate(_unityState.HierarchyItemAtom),
                AtomType.StringPropertyInput => Object.Instantiate(_unityState.StringInputAtom),
                AtomType.NumberPropertyInput => Object.Instantiate(_unityState.NumberInputAtom),
                AtomType.BooleanPropertyInput => Object.Instantiate(_unityState.BooleanInputAtom),
                AtomType.Vector3PropertyInput => Object.Instantiate(_unityState.Vector3InputAtom),
                AtomType.ContextMenu => Object.Instantiate(_unityState.ContextMenuAtom),
                AtomType.ContextMenuItem => Object.Instantiate(_unityState.ContextMenuItemAtom),
                AtomType.ContextSubmenuItem => Object.Instantiate(_unityState.ContextSubmenuItemAtom),
                AtomType.ContextMenuSpacerItem => Object.Instantiate(_unityState.ContextMenuSpacerItemAtom),
                _ => null
            };
        }

        private void ReturnAtomsToPool(GameObject objects)
        {
            Object.Destroy(objects);
        }

        public class Factory : PlaceholderFactory<UiBuilder> { }
    }
}
