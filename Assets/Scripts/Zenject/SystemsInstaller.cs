using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Zenject;

public class SystemsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(ISceneSaveSystem), typeof(ISceneLoadSystem)).To<SceneLoadSaveSystem>().AsSingle();
        //Container.Bind().To<LoadFromVersion1System>().AsSingle();

        Container.Bind<ICommandSystem>().To<CommandSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<CommandFactorySystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<UpdatePropertiesFromUiSystem>().AsSingle();

        Container.Bind(typeof(InputState)).To<InputState>().AsSingle();

        Container.Bind<Interface3DState>().To<Interface3DState>().AsSingle();
    }
}