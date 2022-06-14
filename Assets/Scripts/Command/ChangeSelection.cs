using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SceneState;
using UnityEditor;
using UnityEngine;

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

    }
}
