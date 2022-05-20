using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Utility;
using UnityEngine;

public class MainSceneVisuals : MonoBehaviour, ISetupSceneEventListeners
{
    [SerializeField]
    private GameObject _entityVisualsPrefab;

    public void SetupSceneEventListeners()
    {
        // when there is a scene loaded, add the visuals updater
        EditorStates.CurrentSceneState.CurrentScene?
            .HierarchyChangedEvent.AddListener(UpdateVisuals);

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        var scene = EditorStates.CurrentSceneState.CurrentScene;
        if (scene == null)
            return;

        // TODO: be smarter about caching and stuff
        foreach (var child in transform.GetChildren())
        {
            Destroy(child.gameObject);
        }

        List<EntityVisuals> visuals = new List<EntityVisuals>();

        foreach (var entity in scene.AllEntities.Select(e => e.Value))
        {
            var newEntityVisualsGameObject = Instantiate(_entityVisualsPrefab, transform);
            var newEntityVisuals = newEntityVisualsGameObject.GetComponent<EntityVisuals>();
            newEntityVisuals.Id = entity.Id;

            visuals.Add(newEntityVisuals);

            newEntityVisuals.UpdateVisuals();
        }

        foreach (var visual in visuals)
        {
            var parent = scene.GetEntityFormId(visual.Id).Parent; // look, if the actual entity of the visual has a parent

            if (parent != null)
                // set the transforms parent to the transform of the parent visual
                visual.transform.SetParent(visuals.Find(v => v.Id == parent.Id).transform,true);
        }
        
    }
}
