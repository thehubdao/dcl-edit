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
            StringPropertyInput,
            NumberPropertyInput
        }
        
        private struct UiAtom
        {
            public AtomType Type;
            public int Height;
            public Func<GameObject> MakeGameObject;
        }

        private readonly List<UiAtom> _atoms = new List<UiAtom>();

        public UiBuilder Title(string title)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Title,
                Height = 130,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.Title);

                    var text = go.GetComponent<TextMeshProUGUI>();

                    text.text = title;

                    return go;
                }
            });

            return this;
        }

        public UiBuilder Text(string text)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Title,
                Height = 50,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.Text);

                    var tmpText = go.GetComponent<TextMeshProUGUI>();

                    tmpText.text = text;

                    return go;
                }
            });

            return this;
        }

        public UiBuilder Spacer(int height)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.Spacer,
                Height = height,
                MakeGameObject = () =>
                {
                    var gameObject = new GameObject("Spacer");
                    gameObject.AddComponent<RectTransform>();
                    return gameObject;
                }
            });

            return this;
        }

        public UiBuilder StringPropertyInput(string name, string placeholder, string currentContents)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.StringPropertyInput,
                Height = 50,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.StringPropertyInput);

                    var stringProperty = go.GetComponent<StringPropertyHandler>();

                    stringProperty.propertyNameText.text = name;
                    stringProperty.stingInput.SetCurrentText(currentContents);
                    stringProperty.stingInput.SetPlaceHolder(placeholder);
                        
                    return go;
                }
            });

            return this;
        }

        public UiBuilder NumberPropertyInput(string name, string placeholder, float currentContents)
        {
            _atoms.Add(new UiAtom
            {
                Type = AtomType.NumberPropertyInput,
                Height = 50,
                MakeGameObject = () =>
                {
                    var go = GetAtomObjectFromPool(AtomType.NumberPropertyInput);

                    var numberProprerty = go.GetComponent<NumberPropertyHandler>();

                    numberProprerty.propertyNameText.text = name;
                    numberProprerty.numberInput.SetCurrentNumber(currentContents);
                    numberProprerty.numberInput.TextInputHandler.SetPlaceHolder(placeholder);

                    return go;
                }
            });

            return this;
        }

        public void ClearAndMake(GameObject parent)
        {
            Clear(parent);
            Make(parent);
        }

        private void Clear(GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                ReturnAtomsToPool(child.gameObject);
            }
        }

        private void Make(GameObject parent)
        {
            var heightCounter = 0;
            foreach (var atom in _atoms)
            {
                var go = atom.MakeGameObject();

                var tf = go.GetComponent<RectTransform>();

                tf.SetParent(parent.transform);

                tf.offsetMin = Vector2.zero;
                tf.offsetMax = Vector2.zero;

                tf.anchoredPosition = new Vector3(0, heightCounter, 0);

                tf.sizeDelta = new Vector2(tf.sizeDelta.x, atom.Height);


                heightCounter -= atom.Height;
            }

            var parentRectTransform = parent.GetComponent<RectTransform>();
            parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, -heightCounter);
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
                AtomType.StringPropertyInput => Object.Instantiate(unityState.StringInputAtom),
                AtomType.NumberPropertyInput => Object.Instantiate(unityState.NumberInputAtom),
                _ => null
            };
        }

        private void ReturnAtomsToPool(GameObject objects)
        {
            Object.Destroy(objects);
        }
    }
}
