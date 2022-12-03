using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Visuals.NewUiBuilder;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class AtomGameObject
    {
        public NewUiBuilder.PooledObject gameObject;
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

        /// <summary>
        /// Update the Atom given new Data
        /// </summary>
        /// <param name="newData">the data, that the atom should represent now</param>
        /// <param name="newPosition">the position of the top corner with int its parent GameObject</param>
        /// <returns>when the Position or Height has changed, return true. This can also happen, when a different Position was given in the newPosition parameter</returns>
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
}