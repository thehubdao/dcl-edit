using Assets.Scripts.Events;
using Assets.Scripts.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class EnvironmentVisuals : MonoBehaviour
    {
        private EditorEvents editorEvents;
        private SettingsSystem settingsSystem;

        [SerializeField] private GameObject infiniteGround;
        [SerializeField] private GameObject groundGrid;

        [Inject]
        private void Construct(EditorEvents editorEvents, SettingsSystem settingsSystem)
        {
            this.editorEvents = editorEvents;
            this.settingsSystem = settingsSystem;
        }

        private void OnEnable()
        {
            editorEvents.onSettingsChangedEvent += UpdateGrid;
            UpdateGrid();
        }

        private void OnDisable()
        {
            editorEvents.onSettingsChangedEvent -= UpdateGrid;
        }

        private void UpdateGrid()
        {
            int setting = settingsSystem.groundGridSetting.Get();
            infiniteGround.SetActive(setting == 1);
            groundGrid.SetActive(setting == 2);
        }
    }
}