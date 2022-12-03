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
}