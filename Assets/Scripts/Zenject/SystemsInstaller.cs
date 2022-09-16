using Assets.Scripts.System;
using UnityEngine;
using Zenject;

public class SystemsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(ISceneLoadSystem), typeof(ISceneSaveSystem)).To<SceneLoadSaveSystem>().AsSingle();
    }
}