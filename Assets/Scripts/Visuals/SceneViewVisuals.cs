using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.Profiling;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class SceneViewVisuals : MonoBehaviour
    {
        // Dependencies
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;
        private MainSceneVisuals.Factory mainSceneVisualsFactory;
        private SceneManagerState sceneManagerState;

        [Inject]
        private void Construct(EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem,
            MainSceneVisuals.Factory mainSceneVisualsFactory,
            SceneManagerState sceneManagerState)
        {
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;
            this.mainSceneVisualsFactory = mainSceneVisualsFactory;
            this.sceneManagerState = sceneManagerState;
            SetupEventListeners();
        }

        private void OnEnable()
        {
            sceneManagerState.currentSceneIndex.OnValueChanged += SubscribeToNewScene;
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged += UpdateVisuals;
            UpdateVisuals();
        }

        private void OnDisable()
        {
            sceneManagerState.currentSceneIndex.OnValueChanged -= SubscribeToNewScene;
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged -= UpdateVisuals;
        }

        private void SubscribeToNewScene()
        {
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged -= UpdateVisuals;
            sceneManagerSystem.GetCurrentScene().SelectionState.PrimarySelectedEntity.OnValueChanged += UpdateVisuals;
        }

        private void SetupEventListeners()
        {
            editorEvents.onHierarchyChangedEvent += UpdateVisuals;
            editorEvents.onAssetDataUpdatedEvent += _ => UpdateVisuals();
            editorEvents.onAssetMetadataCacheUpdatedEvent += UpdateVisuals;

            UpdateVisuals();
        }

        private MainSceneVisuals currentMainSceneVisuals = null;

        private void UpdateVisuals()
        {
            Profiler.BeginSample("SceneViewVisuals");

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

            Profiler.EndSample();
        }
    }
}
