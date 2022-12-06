using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class PanelWithBorderAtom : PanelAtom
    {
        public new class Data : PanelAtom.Data
        {
        }

        protected override AtomGameObject MakeNewAtomGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.PanelWithBorder);
            atomObject.height = 40;
            atomObject.position = -1;
            return atomObject;
        }

        protected override int totalBorderHeight { get; set; } = 40;

        public PanelWithBorderAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class PanelWithBorderPanelHelper
    {
        public static PanelWithBorderAtom.Data AddPanelWithBorder(this PanelAtom.Data panelAtomData, [CanBeNull] AtomDataList childDates = null)
        {
            var data = new PanelWithBorderAtom.Data
            {
                childDates = childDates ?? new AtomDataList()
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}