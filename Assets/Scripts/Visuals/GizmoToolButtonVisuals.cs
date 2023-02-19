using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.Interaction;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GizmoToolButtonVisuals : MonoBehaviour
    {
        [SerializeField]
        private GizmoToolButtonInteraction gizmoToolButtonInteraction;

        [SerializeField]
        private List<SceneViewButtonHandler> buttons;

        // Dependencies
        private EditorEvents editorEvents;
        private GizmoToolSystem gizmoToolSystem;

        [Inject]
        private void Construct(EditorEvents editorEvents, GizmoToolSystem gizmoToolSystem)
        {
            this.editorEvents = editorEvents;
            this.gizmoToolSystem = gizmoToolSystem;

            SetupListeners();
            SetupButtonCallbacks();
        }

        private void SetupListeners()
        {
            editorEvents.onUpdateSceneViewButtons += UpdateVisuals;

            UpdateVisuals();
        }

        private void SetupButtonCallbacks()
        {
            foreach (var button in buttons)
            {
                button.SetButtonAction(() => { gizmoToolButtonInteraction.ToolButtonPressed(button.settingToSwitch); });
            }
        }

        private void UpdateVisuals()
        {
            foreach (var button in buttons)
            {
                var shouldBeInteractable = button.settingToSwitch switch
                {
                    GizmoToolButtonInteraction.SettingToSwitch.GizmoToolModeTranslate => gizmoToolSystem.gizmoToolMode != GizmoToolSystem.ToolMode.Translate,
                    GizmoToolButtonInteraction.SettingToSwitch.GizmoToolModeRotate => gizmoToolSystem.gizmoToolMode != GizmoToolSystem.ToolMode.Rotate,
                    GizmoToolButtonInteraction.SettingToSwitch.GizmoToolModeScale => gizmoToolSystem.gizmoToolMode != GizmoToolSystem.ToolMode.Scale,
                    GizmoToolButtonInteraction.SettingToSwitch.GizmoToolRelationLocal => gizmoToolSystem.gizmoToolContext != GizmoToolSystem.ToolContext.Local,
                    GizmoToolButtonInteraction.SettingToSwitch.GizmoToolRelationGlobal => gizmoToolSystem.gizmoToolContext != GizmoToolSystem.ToolContext.Global,
                    GizmoToolButtonInteraction.SettingToSwitch.GizmoToolSnappingToggle => true,
                    _ => throw new ArgumentOutOfRangeException()
                };
                button.SetInteractability(shouldBeInteractable);

                if (button.settingToSwitch == GizmoToolButtonInteraction.SettingToSwitch.GizmoToolSnappingToggle)
                {
                    var toggleButton = button.GetComponent<ToggleSceneViewButtonHandler>();
                    toggleButton.SetOnOff(gizmoToolSystem.isToolSnapping);
                }
            }
        }
    }
}