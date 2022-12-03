using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Visuals.NewUiBuilder;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
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
            NewUiBuilder.Stats.atomsUpdatedCount++;

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
                gameObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Panel),
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
}