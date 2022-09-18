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

        // QuickFix for static injection TODO: remove when Update Properties from UI System gets its proper injection update
        Container.BindInterfacesAndSelfTo<UpdatePropertiesFromUiSystem>().AsSingle().NonLazy();
    }
}