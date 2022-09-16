using Assets.Scripts.System;
using UnityEngine;
using Zenject;

public class SystemsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(ISceneSaveSystem), typeof(ISceneLoadSystem)).To<SceneLoadSaveSystem>().AsSingle();
        //Container.Bind().To<LoadFromVersion1System>().AsSingle();
    }
}