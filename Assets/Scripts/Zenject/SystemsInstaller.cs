using Assets.Scripts.EditorState;
using Assets.Scripts.Interaction;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using UnityEngine;
using Zenject;

public class SystemsInstaller : MonoInstaller
{
    [SerializeField]
    private bool _loadSceneFromVersion1 = false;

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

        Container.BindInterfacesAndSelfTo<SceneState>().AsSingle();

        Container.BindInterfacesAndSelfTo<GizmoState>().AsSingle();

        Container.BindInterfacesAndSelfTo<UnityState>().FromComponentOn(gameObject).AsSingle();

        Container.BindInterfacesAndSelfTo<InputHelper>().AsSingle();

        Container.BindFactory<UiBuilder, UiBuilder.Factory>();

        Container.BindInterfacesAndSelfTo<ProjectState>().AsSingle();

        Container.BindInterfacesAndSelfTo<PathState>().AsSingle();
    }
}