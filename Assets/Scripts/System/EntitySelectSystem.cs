using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;

namespace Assets.Scripts.System
{
    public class EntitySelectSystem
    {
        private InputHelper _inputHelper;
        private SceneDirectoryState _sceneDirectoryState;
        private CommandSystem _commandSystem;

        [Inject]
        private void Construct(InputHelper inputHelper, SceneDirectoryState sceneDirectoryState, CommandSystem commandSystem)
        {
            _inputHelper = inputHelper;
            _sceneDirectoryState = sceneDirectoryState;
            _commandSystem = commandSystem;
        }

        public void ClickedOnEntity(Guid entity)
        {
            if (entity == Guid.Empty)
            {
                DeselectAll();
                return;
            }

            if (_inputHelper.GetIsControlPressed())
            {
                SelectAdditional(entity);
                return;
            }

            // else
            SelectSingle(entity);
        }

        public void SelectAdditional(Guid id)
        {
            var scene = _sceneDirectoryState.CurrentScene;
            var selectionCommand = _commandSystem.CommandFactory.CreateChangeSelection(
                GetPrimarySelectionFromScene(),
                GetSecondarySelectionFromScene(),
                id,
                scene.SelectionState.AllSelectedEntities
                    .Select(e => e?.Id ?? Guid.Empty)
                    .Where(current => current != Guid.Empty && current != id));

            _commandSystem.ExecuteCommand(selectionCommand);
        }

        public void SelectSingle(Guid id)
        {
            var selectionCommand = _commandSystem.CommandFactory.CreateChangeSelection(
                GetPrimarySelectionFromScene(),
                GetSecondarySelectionFromScene(),
                id,
                Array.Empty<Guid>());

            _commandSystem.ExecuteCommand(selectionCommand);
        }

        public void DeselectAll()
        {
            var selectionCommand = _commandSystem.CommandFactory.CreateChangeSelection(
                GetPrimarySelectionFromScene(),
                GetSecondarySelectionFromScene(),
                Guid.Empty,
                Array.Empty<Guid>());

            _commandSystem.ExecuteCommand(selectionCommand);
        }

        public Guid GetPrimarySelectionFromScene()
        {
            return _sceneDirectoryState.CurrentScene!.SelectionState.PrimarySelectedEntity?.Id ?? Guid.Empty;
        }

        public IEnumerable<Guid> GetSecondarySelectionFromScene()
        {
            return _sceneDirectoryState.CurrentScene!.SelectionState.SecondarySelectedEntities.Select(e => e.Id);
        }
    }
}
