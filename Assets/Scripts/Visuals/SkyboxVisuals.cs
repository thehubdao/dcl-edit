using Assets.Scripts.Events;
using Assets.Scripts.System;
using DCL.Skybox;
using UnityEngine;
using Zenject;

public class SkyboxVisuals: MonoBehaviour
{
    [SerializeField]
    private SkyboxConfiguration config;

    [SerializeField]
    private Material mat;

    private Material matCopy;

    [SerializeField]
    private new Light light;

    private EditorEvents editorEvents;
    private SettingsSystem settingsSystem;
    private const int slotCount = 5;
    private const float cycleTime = 24f;

    [Inject]
    private void Construct(EditorEvents editorEvents, SettingsSystem settingsSystem)
    {
        this.editorEvents = editorEvents;
        this.settingsSystem = settingsSystem;

        matCopy = new Material(mat);
        SetupEventListeners();
    }

    private void SetupEventListeners()
    {
        editorEvents.onSettingsChangedEvent += UpdateVisuals;
    }

    private void UpdateVisuals()
    {
        ConfigSkyBox(settingsSystem.skyboxTime.Get());
    }
    
    private void ConfigSkyBox(float daytime)
    {
        config.ApplyOnMaterial(matCopy, daytime, daytime/24, slotCount, light, cycleTime);
        RenderSettings.skybox = matCopy;
    }
}
