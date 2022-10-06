using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Visuals;
using Assets.Scripts.Visuals.UiHandler;
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
        _events.onSelectionChangedEvent += UpdateVisuals;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        var uiBuilder = _uiBuilderFactory.Create();

        MakeHierarchyItemsRecursive(uiBuilder, 0, _sceneDirectoryState.CurrentScene!.EntitiesInSceneRoot);

        uiBuilder.Spacer(300);

        uiBuilder.ClearAndMake(_content);
    }

    private void MakeHierarchyItemsRecursive(UiBuilder uiBuilder, int level, IEnumerable<DclEntity> entities)
    {
        foreach (var entity in entities)
        {
            var isPrimarySelection = _sceneDirectoryState.CurrentScene!.SelectionState.PrimarySelectedEntity == entity;

            var isSecondarySelection = _sceneDirectoryState.CurrentScene!.SelectionState.SecondarySelectedEntities.Contains(entity);

            var style =
                isPrimarySelection ?
                    TextHandler.TextStyle.PrimarySelection :
                    isSecondarySelection ?
                        TextHandler.TextStyle.SecondarySelection :
                        TextHandler.TextStyle.Normal;

            uiBuilder.HierarchyItem(entity.ShownName, level, entity.Children.Any(), true, style, new HierarchyItemHandler.UiHierarchyItemActions
            {
                OnArrowClick = () => { Debug.Log("arrow"); },
                OnNameClick = () => { Debug.Log("name"); }
            });

            MakeHierarchyItemsRecursive(uiBuilder, level + 1, entity.Children);
        }
    }
}
