using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiSettingsVisuals : MonoBehaviour
    {
        [SerializeField]
        private GameObject content;

        // Dependencies
        private EditorEvents editorEvents;
        private SettingsSystem settingsSystem;
        private UiBuilder.UiBuilder uiBuilder;
        private UnityState unityState;
        private InputState inputState;

        [Inject]
        private void Construct(
            EditorEvents editorEvents,
            SettingsSystem settingsSystem,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            UnityState unityState,
            InputState inputState)
        {
            this.editorEvents = editorEvents;
            this.settingsSystem = settingsSystem;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.unityState = unityState;
            this.inputState = inputState;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            editorEvents.onSettingsChangedEvent += SetDirty;
            unityState.StartCoroutine(DelayedUpdate()); // Unity state is guarantied to be available and active
        }

        IEnumerator DelayedUpdate() // There are some problems with Zenject, when using the UiBuilder in the first frame
        {
            yield return null;
            SetDirty();
        }

        private bool _dirty;

        void SetDirty()
        {
            _dirty = true;
        }

        void LateUpdate()
        {
            if (_dirty)
            {
                _dirty = false;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            if (inputState.InState == InputState.InStateType.UiInput)
            {
                return;
            }


            var settingsPanel = new PanelAtom.Data();

            // User settings
            var userSettingsPanel = settingsPanel.AddPanelWithBorder();

            userSettingsPanel.AddPanelHeader("User Settings");

            // Ui scale
            userSettingsPanel.AddNumberProperty(
                "Ui Scale",
                "Ui Scale (default: 1)",
                settingsSystem.uiScalingFactor.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.uiScalingFactor.Set
                });

            // mouse sensitivity
            userSettingsPanel.AddNumberProperty(
                "Mouse Sensitivity",
                "Mouse Sensitivity (default: 1)",
                settingsSystem.mouseSensitivity.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.mouseSensitivity.Set
                });

            // Gizmo Size
            userSettingsPanel.AddNumberProperty(
                "Gizmo Size",
                "Gizmo Size (default: 1)",
                settingsSystem.gizmoSize.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.gizmoSize.Set
                });

            // Maximal frame rate
            userSettingsPanel.AddNumberProperty(
                "Maximal Frame Rate",
                "Maximal Frame Rate (default: 120)",
                settingsSystem.applicationTargetFramerate.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = f => settingsSystem.applicationTargetFramerate.Set((int) f)
                });


            // Gizmo Tool Snapping

            var gizmoToolSnappingPanel = settingsPanel.AddPanelWithBorder();

            gizmoToolSnappingPanel.AddPanelHeader("Gizmo Tool Snapping");

            // translate
            gizmoToolSnappingPanel.AddNumberProperty(
                "Translate Step",
                "Translate (default: 0.25)",
                settingsSystem.gizmoToolTranslateSnapping.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.gizmoToolTranslateSnapping.Set
                });

            // rotate
            gizmoToolSnappingPanel.AddNumberProperty(
                "Rotate Step",
                "Rotate (default: 15)",
                settingsSystem.gizmoToolRotateSnapping.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.gizmoToolRotateSnapping.Set
                });

            // scale
            gizmoToolSnappingPanel.AddNumberProperty(
                "Scale Step",
                "Scale (default: 0.25)",
                settingsSystem.gizmoToolScaleSnapping.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.gizmoToolScaleSnapping.Set
                });
            
            
            // Environment Settings

            var environmentPanel = settingsPanel.AddPanelWithBorder();

            environmentPanel.AddPanelHeader("Environment");

            // skybox
            environmentPanel.AddNumberProperty(
                "Time of day",
                "Time (default: 12)",
                settingsSystem.skyboxTime.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = settingsSystem.skyboxTime.Set
                });

            gizmoToolSnappingPanel.AddNumberProperty(
                "Grid Selection",
                "Grid (default: 1)",
                settingsSystem.groundGridSetting.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = (value) => settingsSystem.groundGridSetting.Set((int)value)
                });

            gizmoToolSnappingPanel.AddNumberProperty(
                "Gridsize Selection",
                "Gridsize (default: 16)",
                settingsSystem.groundGridSizeSetting.Get(),
                new StringPropertyAtom.UiPropertyActions<float>
                {
                    OnSubmit = (value) => settingsSystem.groundGridSizeSetting.Set((int)value)
                });

            // Version number
            settingsPanel.AddSpacer(100);
            settingsPanel.AddText($"dcl-edit version: {Application.version}");

            uiBuilder.Update(settingsPanel);
        }
    }
}