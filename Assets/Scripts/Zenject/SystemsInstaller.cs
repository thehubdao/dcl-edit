using Assets.Scripts.EditorState;
using Assets.Scripts.System;
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
            Container
                .Bind(typeof(ISceneSaveSystem), typeof(ISceneLoadSystem))
                .To<SceneLoadSaveSystem>().AsSingle();
        }

        Container.BindInterfacesAndSelfTo<CommandSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CommandFactorySystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<UpdatePropertiesFromUiSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<InputState>().AsSingle();

        Container.BindInterfacesAndSelfTo<Interface3DState>().AsSingle();

        Container.BindInterfacesAndSelfTo<SetupSceneSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CameraSystem>().AsSingle();
    }
}