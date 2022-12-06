using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Command
{
    public class AddEntity : SceneState.Command
    {
        private readonly string name;

        private readonly Guid parent;

        private readonly Guid id;

        private readonly SelectionUtility.SelectionWrapper oldSelection;

        public AddEntity(Guid oldPrimarySelection, IEnumerable<Guid> oldSecondarySelection, string name = "", Guid? parent = default)
        {
            this.name = name;
            this.parent = parent ?? default;
            id = Guid.NewGuid();
            oldSelection = new SelectionUtility.SelectionWrapper
            {
                Primary = oldPrimarySelection,
                Secondary = oldSecondarySelection.ToList()
            };
        }

        public override string Name => "Add Entity";
        public override string Description => $"Adding Entity \"{name}\" with id \"{id.Shortened()}\"" + (parent != default ? $" as Child to {parent.Shortened()}" : "");

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var e = EntityUtility.AddEntity(sceneState, id, name, parent);

            EntityUtility.AddDefaultTransformComponent(e);
            SelectionUtility.SetSelection(sceneState, id);

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.DeleteEntity(sceneState, id);
            SelectionUtility.SetSelection(sceneState, oldSelection);

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}