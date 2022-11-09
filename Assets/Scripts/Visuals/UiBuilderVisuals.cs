using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Assets.Scripts.Visuals.UiHandler;
using TMPro;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Visuals
{
    public class UiBuilderVisuals : IDisposable
    {
        private GameObject GetAtomObjectFromPool(UiBuilderAtom.AtomType type)
        {
            var atomObject = type switch
            {
                UiBuilderAtom.AtomType.Title => Object.Instantiate(_unityState.TitleAtom),
                UiBuilderAtom.AtomType.Text => Object.Instantiate(_unityState.TextAtom),
                UiBuilderAtom.AtomType.Panel => Object.Instantiate(_unityState.PanelAtom),
                UiBuilderAtom.AtomType.PanelHeader => Object.Instantiate(_unityState.PanelHeaderAtom),
                UiBuilderAtom.AtomType.HierarchyItem => Object.Instantiate(_unityState.HierarchyItemAtom),
                UiBuilderAtom.AtomType.StringPropertyInput => Object.Instantiate(_unityState.StringInputAtom),
                UiBuilderAtom.AtomType.NumberPropertyInput => Object.Instantiate(_unityState.NumberInputAtom),
                UiBuilderAtom.AtomType.BooleanPropertyInput => Object.Instantiate(_unityState.BooleanInputAtom),
                UiBuilderAtom.AtomType.Vector3PropertyInput => Object.Instantiate(_unityState.Vector3InputAtom),
                _ => throw new NotImplementedException()
            };

            var atomHandler = atomObject.GetComponent<AtomHandler>();

            _usedAtoms.Add(atomHandler);

            return atomObject;
        }

        private void ReturnAtomToPool(AtomHandler atom)
        {
            Object.Destroy(atom);
        }

        private void Clear()
        {
            foreach (var atom in _usedAtoms)
            {
                ReturnAtomToPool(atom);
            }

            _currentHeight = 0f;
        }

        // Dependencies
        private UiBuilderState _builderState;
        private EditorEvents _events;
        private UnityState _unityState;
        private UiBuilderVisuals.Factory _buildFactory;


        [Inject]
        public void Construct(UiBuilderState builderState, EditorEvents events, UnityState unityState, UiBuilderVisuals.Factory builderFactory)
        {
            _builderState = builderState;
            _events = events;
            _unityState = unityState;
            _buildFactory = builderFactory;

            // setup event listener
            events.onUiBuilderChangedEvent += TryUpdateVisuals;
        }

        private readonly EventDependentTypes.UiBuilderSetupKey _key;
        private readonly GameObject _contents;

        private readonly List<AtomHandler> _usedAtoms = new List<AtomHandler>();

        private float _currentHeight = 0f;

        public UiBuilderVisuals(EventDependentTypes.UiBuilderSetupKey builderKey, GameObject contents)
        {
            _key = builderKey;
            _contents = contents;
        }

        private void TryUpdateVisuals(EventDependentTypes.UiBuilderSetupKey key)
        {
            if (key == _key)
            {
                UpdateVisuals();
            }
        }

        public void UpdateVisuals()
        {
            Clear();

            var uiBuilderSetup = _builderState.GetBuilderSetup(_key);

            foreach (var atom in uiBuilderSetup.atoms)
            {
                switch (atom)
                {
                    case UiTitleAtom uiTitleAtom:
                        UpdateTitleAtom(uiTitleAtom, ref _currentHeight);
                        break;
                    case UiTextAtom uiTextAtom:
                        UpdateTextAtom(uiTextAtom, ref _currentHeight);
                        break;
                    case UiSpacerAtom uiSpacerAtom:
                        UpdateSpacerAtom(uiSpacerAtom, ref _currentHeight);
                        break;
                    case UiPanelAtom uiPanelAtom:
                        UpdatePanelAtom(uiPanelAtom, ref _currentHeight);
                        break;
                    case UiPanelHeaderAtom uiPanelHeaderAtom:
                        UpdatePanelHeaderAtom(uiPanelHeaderAtom, ref _currentHeight);
                        break;
                    case UiHierarchyItemAtom uiHierarchyItemAtom:
                        //UpdateHierarchyItemAtom(uiHierarchyItemAtom, ref _currentHeight);
                        break;
                    case UiStringPropertyInputAtom uiStringPropertyInputAtom:
                        //UpdateStringPropertyInputAtom(uiStringPropertyInputAtom, ref _currentHeight);
                        break;
                    case UiNumberPropertyInputAtom uiNumberPropertyInputAtom:
                        //UpdateNumberPropertyInputAtom(uiNumberPropertyInputAtom, ref _currentHeight);
                        break;
                    case UiBooleanPropertyInputAtom uiBooleanPropertyInputAtom:
                        //UpdateBooleanPropertyInputAtom(uiBooleanPropertyInputAtom, ref _currentHeight);
                        break;
                    case UiVector3PropertyInputAtom uiVector3PropertyInputAtom:
                        //UpdateVector3PropertyInputAtom(uiVector3PropertyInputAtom, ref _currentHeight);
                        break;
                    default:
                        break;
                }
            }
        }


        private void UpdateTitleAtom(UiTitleAtom atom, ref float currentHeight)
        {
            var go = GetAtomObjectFromPool(UiBuilderAtom.AtomType.Title);

            var text = go.GetComponent<TextMeshProUGUI>();

            text.text = atom.title;

            UpdateTransform(go, ref currentHeight, 130);
        }

        private void UpdateTextAtom(UiTextAtom atom, ref float currentHeight)
        {
            var go = GetAtomObjectFromPool(UiBuilderAtom.AtomType.Text);

            var text = go.GetComponent<TextMeshProUGUI>();

            text.text = atom.text;

            UpdateTransform(go, ref currentHeight, 50);
        }

        private void UpdateSpacerAtom(UiSpacerAtom atom, ref float currentHeight)
        {
            var go = new GameObject("Spacer");
            go.AddComponent<RectTransform>();

            UpdateTransform(go, ref currentHeight, atom.height);
        }

        private void UpdatePanelAtom(UiPanelAtom atom, ref float currentHeight)
        {
            var go = GetAtomObjectFromPool(UiBuilderAtom.AtomType.Panel);

            var panel = go.GetComponent<PanelHandler>();

            var panelBuilder = _buildFactory.Create(atom.contentKey, panel.Content);

            panelBuilder.UpdateVisuals();

            UpdateTransform(go, ref currentHeight, panelBuilder._currentHeight);
        }

        private void UpdatePanelHeaderAtom(UiPanelHeaderAtom atom, ref float currentHeight)
        {
            var go = GetAtomObjectFromPool(UiBuilderAtom.AtomType.PanelHeader);
            var handler = go.GetComponent<PanelHeaderHandler>();
            handler.Title.text = atom.title;

            if (atom.onClose != null)
            {
                handler.CloseButton.gameObject.SetActive(true);
                handler.CloseButton.onClick.AddListener(() => atom.onClose());
            }
            else
            {
                handler.CloseButton.gameObject.SetActive(false);
            }

            UpdateTransform(go, ref currentHeight, 130);
        }


        private void UpdateTransform(GameObject go, ref float currentHeight, float goHeight)
        {
            var tf = go.GetComponent<RectTransform>();

            tf.SetParent(_contents.transform);

            tf.offsetMin = Vector2.zero;
            tf.offsetMax = Vector2.zero;

            tf.anchoredPosition = new Vector2(0, currentHeight);

            tf.sizeDelta = new Vector2(tf.sizeDelta.x, goHeight);

            currentHeight += goHeight;
        }

        public void Dispose()
        {
            _events.onUiBuilderChangedEvent -= TryUpdateVisuals;
        }

        public class Factory : PlaceholderFactory<EventDependentTypes.UiBuilderSetupKey, GameObject, UiBuilderVisuals>
        {
        }
    }
}