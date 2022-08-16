using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Visuals.PropertyHandler;
using TMPro;
using UnityEngine;
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
            StringPropertyInput,
            NumberPropertyInput,
            Vector3PropertyInput
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

        private readonly List<UiAtom> _atoms = new List<UiAtom>();

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

        public UiBuilder StringPropertyInput(string name, string placeholder, string currentContents)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.StringPropertyInput,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.StringPropertyInput);

                    var stringProperty = go.GetComponent<StringPropertyHandler>();

                    stringProperty.propertyNameText.text = name;
                    stringProperty.stingInput.SetCurrentText(currentContents);
                    stringProperty.stingInput.SetPlaceHolder(placeholder);

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder NumberPropertyInput(string name, string placeholder, float currentContents)
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

                    return new MakeGmReturn { go = go, height = 50 };
                }
            });

            return this;
        }

        public UiBuilder Vector3PropertyInput(string name, string[] placeholders, Vector3 currentContents)
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

                    return new MakeGmReturn { go = go, height = 50 };
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
        private GameObject GetAtomObjectFromPool(AtomType type)
        {
            var unityState = EditorStates.CurrentUnityState;

            return type switch
            {
                AtomType.Title => Object.Instantiate(unityState.TitleAtom),
                AtomType.Text => Object.Instantiate(unityState.TextAtom),
                AtomType.Panel => Object.Instantiate(unityState.PanelAtom),
                AtomType.StringPropertyInput => Object.Instantiate(unityState.StringInputAtom),
                AtomType.NumberPropertyInput => Object.Instantiate(unityState.NumberInputAtom),
                AtomType.Vector3PropertyInput => Object.Instantiate(unityState.Vector3InputAtom),
                _ => null
            };
        }

        private void ReturnAtomsToPool(GameObject objects)
        {
            Object.Destroy(objects);
        }
    }
}
