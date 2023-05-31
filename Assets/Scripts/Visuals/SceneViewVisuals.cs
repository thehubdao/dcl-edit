using Assets.Scripts.Events;
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

        private bool dirty = true;

        private void SetDirty()
        {
            dirty = true;
        }

        void LateUpdate()
        {
            if (dirty)
            {
                dirty = false;
                UpdateVisuals();
            }
        }


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
            editorEvents.onHierarchyChangedEvent += SetDirty;
            editorEvents.onSelectionChangedEvent += SetDirty;
            editorEvents.onAssetDataUpdatedEvent += _ => SetDirty();
            editorEvents.onAssetMetadataCacheUpdatedEvent += SetDirty;
            editorEvents.onValueChangedEvent += SetDirty;

            SetDirty();
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
