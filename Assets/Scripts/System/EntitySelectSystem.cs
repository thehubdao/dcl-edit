using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using Zenject;

namespace Assets.Scripts.System
{
    public class EntitySelectSystem
    {
        private InputHelper inputHelper;
        private CommandSystem commandSystem;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            InputHelper inputHelper,
            CommandSystem commandSystem,
            SceneManagerSystem sceneManagerSystem)
        {
            this.inputHelper = inputHelper;
            this.commandSystem = commandSystem;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        public void ClickedOnEntity(Guid entity)
        {
            if (entity == Guid.Empty)
            {
                DeselectAll();
                return;
            }

            // if (inputHelper.GetIsControlPressed())
            // {
            //     SelectAdditional(entity);
            //     return;
            // }

            // else
            SelectSingle(entity);
        }

        public void SelectAdditional(Guid id)
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            if (scene == null)
            {
                return;
            }

            var selectionCommand = commandSystem.CommandFactory.CreateChangeSelection(
                GetPrimarySelectionFromScene(scene),
                GetSecondarySelectionFromScene(scene),
                id,
                scene.SelectionState.AllSelectedEntities
                    .Select(e => e?.Id ?? Guid.Empty)
                    .Where(current => current != Guid.Empty && current != id));

            commandSystem.ExecuteCommand(selectionCommand);
        }

        public void SelectSingle(Guid id)
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            if (scene == null)
            {
                return;
            }

            var selectionCommand = commandSystem.CommandFactory.CreateChangeSelection(
                GetPrimarySelectionFromScene(scene),
                GetSecondarySelectionFromScene(scene),
                id,
                Array.Empty<Guid>());

            commandSystem.ExecuteCommand(selectionCommand);
        }

        public void DeselectAll()
        {
            var scene = sceneManagerSystem.GetCurrentScene();
            if (scene == null)
            {
                return;
            }

            var selectionCommand = commandSystem.CommandFactory.CreateChangeSelection(
                GetPrimarySelectionFromScene(scene),
                GetSecondarySelectionFromScene(scene),
                Guid.Empty,
                Array.Empty<Guid>());

            commandSystem.ExecuteCommand(selectionCommand);
        }

        public Guid GetPrimarySelectionFromScene([NotNull] DclScene scene)
        {
            return scene.SelectionState.PrimarySelectedEntity?.Id ?? Guid.Empty;
        }

        public IEnumerable<Guid> GetSecondarySelectionFromScene([NotNull] DclScene scene)
        {
            return scene.SelectionState.SecondarySelectedEntities.Select(e => e.Id);
        }
    }
}
