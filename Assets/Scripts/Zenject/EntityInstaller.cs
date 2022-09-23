using Assets.Scripts.Interaction;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using UnityEngine;
using Zenject;

public class EntityInstaller : MonoInstaller
{
    [SerializeField]
    private GameObject _entityVisualPrefab;

    [SerializeField]
    private GameObject _translateGizmoPrefab;

    [SerializeField]
    private GameObject _rotateGizmoPrefab;

    [SerializeField]
    private GameObject _scaleGizmoPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<EntitySelectInteraction, EntitySelectInteraction.Factory>().FromComponentInNewPrefab(_entityVisualPrefab);

        Container.BindFactory<GizmoSizeFixerSystem, GizmoVisuals.TranslateFactory>().FromComponentInNewPrefab(_translateGizmoPrefab).AsSingle();
        Container.BindFactory<GizmoSizeFixerSystem, GizmoVisuals.RotateFactory>().FromComponentInNewPrefab(_rotateGizmoPrefab).AsSingle();
        Container.BindFactory<GizmoSizeFixerSystem, GizmoVisuals.ScaleFactory>().FromComponentInNewPrefab(_scaleGizmoPrefab).AsSingle();
    }
}