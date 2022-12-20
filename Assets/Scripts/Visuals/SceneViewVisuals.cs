using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class SceneViewVisuals : MonoBehaviour
    {
        // Dependencies
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;
        private MainSceneVisuals.Factory mainSceneVisualsFactory;

        [Inject]
        private void Construct(EditorEvents editorEvents, SceneManagerSystem sceneManagerSystem, MainSceneVisuals.Factory mainSceneVisualsFactory)
        {
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;
            this.mainSceneVisualsFactory = mainSceneVisualsFactory;

            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            editorEvents.onHierarchyChangedEvent += UpdateVisuals;
            editorEvents.onSelectionChangedEvent += UpdateVisuals;

            UpdateVisuals();
        }

        private MainSceneVisuals currentMainSceneVisuals = null;

        private void UpdateVisuals()
        {
            var currentScene = sceneManagerSystem.GetCurrentDirectoryState();

            if (currentScene == null)
            {
                return;
            }

            if (currentMainSceneVisuals is null)
            {
                currentMainSceneVisuals = mainSceneVisualsFactory.Create();
                currentMainSceneVisuals.transform.SetParent(transform, false);
            }

            currentMainSceneVisuals.ShowScene(currentScene.id);
        }
    }
}
