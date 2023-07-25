using Assets.Scripts.Interaction;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GizmoModeMenu : MonoBehaviour
{
    private GizmoToolSystem gizmoToolSystem;
    [SerializeField] private SceneViewButtonHandler[] sceneViewButtonHandlers;

    [Inject]
    private void Construct(GizmoToolSystem gizmoToolSystem)
    {
        this.gizmoToolSystem = gizmoToolSystem;
    }

    private void OnEnable()
    {
        UpdateVisuals();
        gizmoToolSystem.gizmoToolModeField.OnValueChanged += UpdateVisuals;
    }

    private void OnDisable()
    {
        gizmoToolSystem.gizmoToolModeField.OnValueChanged -= UpdateVisuals;
    }

    private void UpdateVisuals()
    {
        foreach (var button in sceneViewButtonHandlers)
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
