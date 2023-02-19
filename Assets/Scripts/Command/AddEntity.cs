using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;

namespace Assets.Scripts.Command
{
    public class AddEntity : SceneState.Command
    {
        private readonly EntityPresetState.EntityPreset preset;

        private readonly Guid parent;

        private readonly Guid id;

        private readonly SelectionUtility.SelectionWrapper oldSelection;
        
        private readonly float hierarchyOrder;

        public AddEntity(Guid oldPrimarySelection, IEnumerable<Guid> oldSecondarySelection,
            EntityPresetState.EntityPreset preset, float hierarchyOrder, Guid parent = default)
        {
            this.preset = preset;
            this.parent = parent;
            id = Guid.NewGuid();
            oldSelection = new SelectionUtility.SelectionWrapper
            {
                Primary = oldPrimarySelection,
                Secondary = oldSecondarySelection.ToList()
            };
            this.hierarchyOrder = hierarchyOrder;
        }

        public override string Name => "Add Entity";
        public override string Description => $"Adding Entity \"{preset.name}\" with id \"{id.Shortened()}\"" + (parent != default ? $" as Child to {parent.Shortened()}" : "");

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var entity = EntityUtility.AddEntity(sceneState, id, preset.name, hierarchyOrder, parent);

            EntityUtility.AddDefaultTransformComponent(entity);

            foreach (var component in preset.components)
            {
                EntityUtility.AddComponent(entity, component);
            }

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