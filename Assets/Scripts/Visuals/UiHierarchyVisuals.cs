using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Visuals;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class UiHierarchyVisuals : MonoBehaviour, ISetupSceneEventListeners
{
    [SerializeField]
    private GameObject _content;

    // Dependencies
    private EditorEvents _events;
    private UiBuilder.Factory _uiBuilderFactory;
    private SceneDirectoryState _sceneDirectoryState;

    [Inject]
    private void Construct(EditorEvents events, UiBuilder.Factory uiBuilderFactory, SceneDirectoryState scene)
    {
        _events = events;
        _uiBuilderFactory = uiBuilderFactory;
        _sceneDirectoryState = scene;
    }

    public void SetupSceneEventListeners()
    {
        _events.onHierarchyChangedEvent += UpdateVisuals;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        var uiBuilder = _uiBuilderFactory.Create();

        MakeHierarchyItemsRecursive(uiBuilder, 0, _sceneDirectoryState.CurrentScene!.EntitiesInSceneRoot);

        uiBuilder.ClearAndMake(_content);
    }

    private void MakeHierarchyItemsRecursive(UiBuilder uiBuilder, int level, IEnumerable<DclEntity> entities)
    {
        foreach (var entity in entities)
        {
            uiBuilder.HierarchyItem(entity.ShownName, level, true);

            MakeHierarchyItemsRecursive(uiBuilder, level + 1, entity.Children);
        }
    }
}
