using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiSceneVisuals : MonoBehaviour
    {
        [SerializeField]
        public RectTransform scenePanelTab;
        
        #region Mark for update

        private bool shouldUpdate;

        private void LateUpdate()
        {
            if (!shouldUpdate) return;
            
            UpdateVisuals();
            shouldUpdate = false;
        }

        private void MarkForUpdate()
        {
            shouldUpdate = true;
        }

        #endregion
        
        // Dependencies
        private PanelSystem panelSystem;
        private EditorEvents events;
        private SceneManagerState sceneManagerState;
        private SceneChangeDetectSystem sceneChangeDetectSystem;

        [Inject]
        private void Construct(PanelSystem panelSystem, EditorEvents events, SceneManagerState sceneManagerState, SceneChangeDetectSystem sceneChangeDetectSystem)
        {
            this.panelSystem = panelSystem;
            this.events = events;
            this.sceneManagerState = sceneManagerState;
            this.sceneChangeDetectSystem = sceneChangeDetectSystem;
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            events.OnCurrentSceneChangedEvent += MarkForUpdate;
            
            MarkForUpdate();
        }

        private void UpdateVisuals()
        {
            ChangeSceneTabTitleToCurrentSceneTitle();
        }

        private void ChangeSceneTabTitleToCurrentSceneTitle()
        {
            var currentDirectoryState = sceneManagerState.GetCurrentDirectoryState();

            if (currentDirectoryState != null)
            {
                string title = currentDirectoryState.name;
                if (sceneChangeDetectSystem.HasSceneChanged()) title += "*";
                panelSystem.ChangePanelTabTitle(scenePanelTab, title);
            }
        }
    }
}