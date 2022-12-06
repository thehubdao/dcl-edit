using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.NewUiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class ContextMenuSpacerAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public Guid menuId;
            public ContextMenuSystem contextMenuSystem;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is ContextMenuSpacerAtom.Data otherContextMenuText))
                {
                    return false;
                }

                return
                    menuId.Equals(otherContextMenuText.menuId) &&
                    contextMenuSystem.Equals(otherContextMenuText.contextMenuSystem);
            }
        }

        protected Data data;

        public override bool Update(Atom.Data newData, int newPosition)
        {
            NewUiBuilder.Stats.atomsUpdatedCount++;

            var posHeightHasChanged = false;
            var newContextMenuSpacerData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
                posHeightHasChanged = true;
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newContextMenuSpacerData.Equals(data))
            {
                // Update data
                var hoverHandler = gameObject.gameObject.GetComponent<ContextMenuHoverHandler>();
                hoverHandler.OnHoverAction = () => newContextMenuSpacerData.contextMenuSystem.CloseMenusUntil(newContextMenuSpacerData.menuId);

                data = newContextMenuSpacerData;
            }

            // Stage 3: Check for changes in Position and Height and update, if it has changed
            if (newPosition != gameObject.position)
            {
                UpdatePositionAndSize(newPosition, gameObject.height);
                posHeightHasChanged = true;
            }

            return posHeightHasChanged;
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.ContextMenuSpacerItem);
            atomObject.height = 15;
            atomObject.position = -1;
            return atomObject;
        }


        public ContextMenuSpacerAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class ContextMenuSpacerPanelHelper
    {
        public static ContextMenuSpacerAtom.Data AddContextMenuSpacer(this PanelAtom.Data panelAtomData, Guid menuId, ContextMenuSystem contextMenuSystem)
        {
            var data = new ContextMenuSpacerAtom.Data
            {
                menuId = menuId,
                contextMenuSystem = contextMenuSystem
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}