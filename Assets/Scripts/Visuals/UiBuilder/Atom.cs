using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AtomGameObject
    {
        public GameObject gameObject;
        public UiBuilder.AtomType atomType;
    }


    public abstract class Atom
    {
        public UiBuilder uiBuilder;
        public AtomGameObject gameObject;

        protected Atom(UiBuilder uiBuilder)
        {
            this.uiBuilder = uiBuilder;
        }

        /// <summary>
        /// Update the atom with the given data
        /// </summary>
        /// <param name="newData">the data, that the atom should represent now</param>
        /// <returns>when the Position or Height has changed, return true. This can also happen, when a different Position was given in the newPosition parameter</returns>
        public abstract void Update([NotNull] Data newData);

        public virtual void Remove()
        {
            uiBuilder.ReturnAtomsToPool(gameObject);
        }

        public abstract class Data
        {
            public abstract bool Equals(Data other);
        }

        /// <summary>
        /// Check if Atom Type and Data Type match
        /// </summary>
        /// <param name="data">the data to check</param>
        /// <returns>Returns true, if the data is from the same type as this Atom</returns>
        public bool DoesDataTypeMatch(Data data)
        {
            return this switch
            {
                PanelAtom _ => typeof(PanelAtom.Data) == data.GetType(),
                TitleAtom _ => typeof(TitleAtom.Data) == data.GetType(),
                TextAtom _ => typeof(TextAtom.Data) == data.GetType(),
                _ => false
            };
        }
    }

    public class AtomDataList : List<Atom.Data>
    {
    }
}