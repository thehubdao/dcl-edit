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
                new ValueBindStrategy<float>(settingsSystem.uiScalingFactor.Get, onValueSubmitted: settingsSystem.uiScalingFactor.Set));

            // mouse sensitivity
            userSettingsPanel.AddNumberProperty(
                "Mouse Sensitivity",
                "Mouse Sensitivity (default: 1)",
                new ValueBindStrategy<float>(settingsSystem.mouseSensitivity.Get, onValueSubmitted: settingsSystem.mouseSensitivity.Set));


            // Gizmo Size
            userSettingsPanel.AddNumberProperty(
                "Gizmo Size",
                "Gizmo Size (default: 1)",
                new ValueBindStrategy<float>(settingsSystem.gizmoSize.Get, onValueSubmitted: settingsSystem.gizmoSize.Set));

            // Maximal frame rate
            userSettingsPanel.AddNumberProperty(
                "Maximal Frame Rate",
                "Maximal Frame Rate (default: 120)",
                new ValueBindStrategy<float>(()=>settingsSystem.applicationTargetFramerate.Get(), onValueSubmitted: value => settingsSystem.applicationTargetFramerate.Set((int)value)));
                

            // Gizmo Tool Snapping

            var gizmoToolSnappingPanel = settingsPanel.AddPanelWithBorder();

            gizmoToolSnappingPanel.AddPanelHeader("Gizmo Tool Snapping");

            // translate
            gizmoToolSnappingPanel.AddNumberProperty(
                "Translate Step",
                "Translate (default: 0.25)",
                new ValueBindStrategy<float>(settingsSystem.gizmoToolTranslateSnapping.Get, onValueSubmitted: settingsSystem.gizmoToolTranslateSnapping.Set));

            // rotate
            gizmoToolSnappingPanel.AddNumberProperty(
                "Rotate Step",
                "Rotate (default: 15)",
                new ValueBindStrategy<float>(settingsSystem.gizmoToolRotateSnapping.Get, onValueChanged: settingsSystem.gizmoToolRotateSnapping.Set));

            // scale
            gizmoToolSnappingPanel.AddNumberProperty(
                "Scale Step",
                "Scale (default: 0.25)",
                new ValueBindStrategy<float>(settingsSystem.gizmoToolScaleSnapping.Get, onValueChanged: settingsSystem.gizmoToolScaleSnapping.Set));

            // Version number
            settingsPanel.AddSpacer(100);
            settingsPanel.AddText($"dcl-edit version: {Application.version}");

            uiBuilder.Update(settingsPanel);
        }
    }
}