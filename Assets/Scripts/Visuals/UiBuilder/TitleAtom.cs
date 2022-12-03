using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Visuals.NewUiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class TitleAtom : TextAtom
    {
        public new class Data : TextAtom.Data
        {
        }

        protected override AtomGameObject MakeNewGameObject()
        {
            return new AtomGameObject
            {
                gameObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Title),
                height = 130,
                position = -1
            };
        }


        public TitleAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }
}