using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.Asteroids;
using Zenject;
using System;
using UnityEngine.UI;

public class MainCanvasVisuals : MonoBehaviour
{
    // Dependencies
    private SettingsSystem _settingsSystem;
    private EditorEvents _editorEvents;

    private CanvasScaler canvasScaler;

    [Inject]
    private void Construct(
        SettingsSystem settingsSystem,
        EditorEvents editorEvents)
    {
        _settingsSystem = settingsSystem;
        _editorEvents = editorEvents;

        canvasScaler = GetComponent<CanvasScaler>();

        // subscribe events
        SetUiScalingFactor();
        _editorEvents.onSettingsChangedEvent += SetUiScalingFactor;
    }

    private void SetUiScalingFactor()
    {
        canvasScaler.scaleFactor = _settingsSystem.uiScalingFactor.Get();
    }
}
