using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Command
{
    public class ChangeSelection : SceneState.Command
    {
        private readonly SelectionUtility.SelectionWrapper oldSelection;
        private readonly SelectionUtility.SelectionWrapper newSelection;

        public ChangeSelection(Guid oldPrimary, IEnumerable<Guid> oldSecondary, Guid newPrimary, IEnumerable<Guid> newSecondary)
        {
            oldSelection = new SelectionUtility.SelectionWrapper
            {
                Primary = oldPrimary,
                Secondary = oldSecondary.ToList()
            };

            newSelection = new SelectionUtility.SelectionWrapper
            {
                Primary = newPrimary,
                Secondary = newSecondary.ToList()
            };
        }

        public override string Name => "Change Selection";
        public override string Description
        {
            get
            {
                if (newSelection.Primary == Guid.Empty)
                    return "Deselecting all Entities";

                var retVal = $"Selecting as primary selection: {GetNameFromGuid(newSelection.Primary)}";

                if (newSelection.Secondary.Count > 0)
                {
                    retVal += " and as secondary selection: ";
                    for (var i = 0; i < newSelection.Secondary.Count; i++)
                    {
                        if (i != 0)
                            retVal += " ,";

                        var guid = newSelection.Secondary[i];

                        retVal += GetNameFromGuid(guid);
                    }
                }

                return retVal;
            }
        }

        private string GetNameFromGuid(Guid id)
        {
            return id.Shortened();
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            SelectionUtility.SetSelection(sceneState, newSelection);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            SelectionUtility.SetSelection(sceneState, oldSelection);
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}
