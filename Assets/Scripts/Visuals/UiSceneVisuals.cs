using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiSceneVisuals : MonoBehaviour
    {
        [SerializeField]
        public RectTransform scenePanelTab;

        // Dependencies
        private PanelSystem panelSystem;
        private SceneManagerState sceneManagerState;
        private SceneChangeDetectSystem sceneChangeDetectSystem;

        [Inject]
        private void Construct(PanelSystem panelSystem, SceneManagerState sceneManagerState, SceneChangeDetectSystem sceneChangeDetectSystem)
        {
            this.panelSystem = panelSystem;
            this.sceneManagerState = sceneManagerState;
            this.sceneChangeDetectSystem = sceneChangeDetectSystem;
        }

        private void OnEnable()
        {
            sceneManagerState.currentSceneIndex.OnValueChanged += UpdateLabel;
            sceneChangeDetectSystem.savedCommandIndexOffset.OnValueChanged += UpdateLabel;
            UpdateLabel();
        }

        private void OnDisable()
        {
            sceneManagerState.currentSceneIndex.OnValueChanged -= UpdateLabel;
            sceneChangeDetectSystem.savedCommandIndexOffset.OnValueChanged -= UpdateLabel;
        }

        private void UpdateLabel()
        {
            var currentDirectoryState = sceneManagerState.GetCurrentDirectoryState();
            if (currentDirectoryState == null) return;
            string title = currentDirectoryState.name;
            if (sceneChangeDetectSystem.HasSceneChanged())
                title += "*";
            panelSystem.ChangePanelTabTitle(scenePanelTab, title);
        }
    }
}