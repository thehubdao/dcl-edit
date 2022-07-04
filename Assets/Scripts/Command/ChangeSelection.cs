using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Command
{
    public class ChangeSelection : SceneState.Command
    {
        private struct SelectionWrapper
        {
            public Guid Primary;
            public List<Guid> Secondary;
        }

        private readonly SelectionWrapper _oldSelection;
        private readonly SelectionWrapper _newSelection;

        public ChangeSelection(Guid oldPrimary, IEnumerable<Guid> oldSecondary, Guid newPrimary, IEnumerable<Guid> newSecondary)
        {
            _oldSelection.Primary = oldPrimary;
            _oldSelection.Secondary = oldSecondary.ToList();
            _newSelection.Primary = newPrimary;
            _newSelection.Secondary = newSecondary.ToList();
        }

        public override string Name => "Change Selection";
        public override string Description
        {
            get
            {
                if(_newSelection.Primary == Guid.Empty)
                    return "Deselecting all Entities";

                var retVal = $"Selecting as primary selection: {GetNameFromGuid(_newSelection.Primary)}";

                if (_newSelection.Secondary.Count > 0)
                {
                    retVal += " and as secondary selection: ";
                    for (var i = 0; i < _newSelection.Secondary.Count; i++)
                    {
                        if (i != 0)
                            retVal += " ,";

                        var guid = _newSelection.Secondary[i];

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

        public override void Do(DclScene sceneState)
        {
            sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityFormId(_newSelection.Primary);
            sceneState.SelectionState.SecondarySelectedEntities.Clear();
            foreach (var secondary in _newSelection.Secondary)
            {
                sceneState.SelectionState.SecondarySelectedEntities.Add(sceneState.GetEntityFormId(secondary));
            }
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        public override void Undo(DclScene sceneState)
        {
            sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityFormId(_oldSelection.Primary);
            sceneState.SelectionState.SecondarySelectedEntities.Clear();
            foreach (var secondary in _oldSelection.Secondary)
            {
                sceneState.SelectionState.SecondarySelectedEntities.Add(sceneState.GetEntityFormId(secondary));
            }
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        public static Guid GetPrimarySelectionFromScene(DclScene sceneState)
        {
            return sceneState.SelectionState.PrimarySelectedEntity?.Id ?? Guid.Empty;
        }

        public static IEnumerable<Guid> GetSecondarySelectionFromScene(DclScene sceneState)
        {
            return sceneState.SelectionState.SecondarySelectedEntities.Select(e => e.Id);
        }

        public static ChangeSelection MakeAdditionalSelectCommand(DclScene scene, Guid idToAdd)
        {
            return new ChangeSelection(
                GetPrimarySelectionFromScene(scene),
                GetSecondarySelectionFromScene(scene),
                idToAdd,
                scene.SelectionState.AllSelectedEntities
                    .Select(e => e?.Id ?? Guid.Empty)
                    .Where(id => id != Guid.Empty && id != idToAdd));
        }

        public static ChangeSelection MakeSingleSelectCommand(DclScene scene, Guid idToSelect)
        {
            if(scene == null)
                return null;

            return new ChangeSelection(
                GetPrimarySelectionFromScene(scene),
                GetSecondarySelectionFromScene(scene),
                idToSelect,
                Array.Empty<Guid>());
        }

    }
}
