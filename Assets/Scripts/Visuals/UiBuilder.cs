using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
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
            Text
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

                tf.localPosition = new Vector3(tf.localPosition.x, heightCounter, tf.localPosition.z);

                tf.SetParent(parent.transform);

                heightCounter += atom.Height;
            }
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
                _ => null
            };
        }

        private void ReturnAtomsToPool(GameObject objects)
        {
            Object.Destroy(objects);
        }
    }
}
