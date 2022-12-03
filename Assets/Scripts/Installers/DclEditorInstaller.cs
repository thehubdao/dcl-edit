using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Interaction;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using UnityEngine;
using Zenject;

public class DclEditorInstaller : MonoInstaller
{
    [Header("Scene Loading and Saving")]
    [SerializeField]
    private bool _loadSceneFromVersion1 = false;

    [Header("Prefabs")]
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
        if (_loadSceneFromVersion1)
        {
            Container.Bind<ISceneLoadSystem>().To<LoadFromVersion1System>().AsSingle();
            Container.Bind<ISceneSaveSystem>().To<SceneLoadSaveSystem>().AsSingle();
        }
        else
        {
            Container.BindInterfacesAndSelfTo<SceneLoadSaveSystem>().AsSingle();
        }

        Container.BindInterfacesAndSelfTo<CommandSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CommandFactorySystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<UpdatePropertiesFromUiSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<InputState>().AsSingle();

        Container.BindInterfacesAndSelfTo<Interface3DState>().AsSingle();

        Container.BindInterfacesAndSelfTo<SetupSceneSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CameraSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ExposeEntitySystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<SetupSceneEventListenersSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<WorkspaceSaveSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ModelCacheSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ModelCacheState>().AsSingle();

        Container.BindInterfacesAndSelfTo<CameraState>().AsSingle();

        Container.BindInterfacesAndSelfTo<SceneDirectoryState>().AsSingle();

        Container.BindInterfacesAndSelfTo<GizmoState>().AsSingle();

        Container.BindInterfacesAndSelfTo<UnityState>().FromComponentOn(gameObject).AsSingle();

        Container.BindInterfacesAndSelfTo<InputHelper>().AsSingle();

        Container.BindFactory<UiBuilder, UiBuilder.Factory>().AsSingle();
        Container.BindFactory<GameObject, NewUiBuilder, NewUiBuilder.Factory>().AsSingle();

        Container.BindInterfacesAndSelfTo<ProjectState>().AsSingle();

        Container.BindInterfacesAndSelfTo<PathState>().AsSingle();

        Container.BindInterfacesAndSelfTo<TypeScriptGenerationSystem>().AsSingle();

        Container.BindFactory<EntitySelectInteraction, EntitySelectInteraction.Factory>().FromComponentInNewPrefab(_entityVisualPrefab);

        Container.BindFactory<GizmoSizeFixerSystem, GizmoVisuals.TranslateFactory>().FromComponentInNewPrefab(_translateGizmoPrefab).AsSingle();

        Container.BindFactory<GizmoSizeFixerSystem, GizmoVisuals.RotateFactory>().FromComponentInNewPrefab(_rotateGizmoPrefab).AsSingle();

        Container.BindFactory<GizmoSizeFixerSystem, GizmoVisuals.ScaleFactory>().FromComponentInNewPrefab(_scaleGizmoPrefab).AsSingle();

        Container.BindInterfacesAndSelfTo<EditorEvents>().AsSingle();

        Container.BindInterfacesAndSelfTo<HierarchyChangeSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<EntitySelectSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<HierarchyExpansionState>().AsSingle();

        Container.BindInterfacesAndSelfTo<SettingsSystem>().AsSingle();

        Container.Bind<ProjectSettingState>().To<ProjectSettingState>().AsSingle();

        Container.Bind<SceneSettingState>().To<SceneSettingState>().AsSingle();

        Container.BindInterfacesAndSelfTo<ContextMenuSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ContextMenuState>().AsSingle();
    }
}