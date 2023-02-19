using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Interaction;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using Assets.Scripts.Visuals.UiBuilder;
using UnityEngine;
using Zenject;

public class DclEditorInstaller : MonoInstaller
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject _entityVisualPrefab;

    [SerializeField]
    private GameObject _translateGizmoPrefab;

    [SerializeField]
    private GameObject _rotateGizmoPrefab;

    [SerializeField]
    private GameObject _scaleGizmoPrefab;

    [SerializeField]
    private GameObject _assetThumbnailGeneratorPrefab;

    [SerializeField]
    private GameObject mainSceneVisualsPrefab;

    [Header("Unity State")]
    [SerializeField]
    private UnityState unityState;


    public override void InstallBindings()
    {
        Container.Bind<LoadFromVersion1System>().To<LoadFromVersion1System>().AsSingle();

        Container.Bind(typeof(ISceneLoadSystem), typeof(ISceneSaveSystem)).To<SceneLoadSaveSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CommandSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CommandFactorySystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<UpdatePropertiesFromUiSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<InputState>().AsSingle();

        Container.BindInterfacesAndSelfTo<Interface3DState>().AsSingle();

        Container.BindInterfacesAndSelfTo<ApplicationSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<CameraSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ExposeEntitySystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<WorkspaceSaveSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ModelCacheSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<ModelCacheState>().AsSingle();

        Container.BindInterfacesAndSelfTo<CameraState>().AsSingle();

        //Container.BindInterfacesAndSelfTo<SceneDirectoryState>().AsSingle();

        Container.BindInterfacesAndSelfTo<GizmoState>().AsSingle();

        Container.BindInterfacesAndSelfTo<UnityState>().FromComponentOn(unityState.gameObject).AsSingle();

        Container.BindInterfacesAndSelfTo<InputHelper>().AsSingle();

        Container.BindFactory<GameObject, UiBuilder, UiBuilder.Factory>().AsSingle();

        Container.BindInterfacesAndSelfTo<ProjectState>().AsSingle();

        Container.BindInterfacesAndSelfTo<PathState>().AsSingle();

        Container.BindInterfacesAndSelfTo<TypeScriptGenerationSystem>().AsSingle();

        Container.BindFactory<EntitySelectInteraction, EntitySelectInteraction.Factory>().FromComponentInNewPrefab(_entityVisualPrefab);

        Container.BindFactory<MainSceneVisuals, MainSceneVisuals.Factory>().FromComponentInNewPrefab(mainSceneVisualsPrefab);

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

        Container.BindInterfacesAndSelfTo<AssetManagerSystem>().AsSingle();

        Container.Bind<IAssetLoaderSystem>().To<FileAssetLoaderSystem>().AsSingle();
        Container.Bind<IAssetLoaderSystem>().To<BuilderAssetLoaderSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<FileAssetLoaderState>().AsTransient();

        Container.BindInterfacesAndSelfTo<AssetBrowserSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<AssetBrowserState>().AsSingle();

        Container.BindInterfacesAndSelfTo<LoadGltfFromFileSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<BuilderAssetLoaderState>().AsSingle();

        Container.BindInterfacesAndSelfTo<WebRequestSystem>().AsSingle();


        Container.BindInterfacesAndSelfTo<SceneJsonReaderSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<NumberInputSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<HierarchyContextMenuSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<AssetThumbnailManagerSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<AssetThumbnailGeneratorSystem>().FromComponentInNewPrefab(_assetThumbnailGeneratorPrefab).AsSingle();

        Container.BindInterfacesAndSelfTo<AssetThumbnailGeneratorState>().AsSingle();

        Container.BindFactory<AssetBrowserButtonHandler, AssetBrowserButtonHandler.Factory>().FromComponentInNewPrefab(unityState.AssetBrowserButtonAtom);

        Container.BindFactory<AssetBrowserFolderHandler, AssetBrowserFolderHandler.Factory>().FromComponentInNewPrefab(unityState.AssetBrowserFolderAtom);

        Container.BindInterfacesAndSelfTo<SceneManagerSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<SceneManagerState>().AsSingle();

        Container.BindInterfacesAndSelfTo<SceneViewSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<MenuBarState>().AsSingle();

        Container.BindInterfacesAndSelfTo<MenuBarSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<AddComponentSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<AvailableComponentsState>().AsSingle();

        Container.BindInterfacesAndSelfTo<CheckVersionSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<EntityPresetSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<EntityPresetState>().AsSingle();

        Container.BindInterfacesAndSelfTo<DialogSystem>().AsSingle();

        Container.BindInterfacesAndSelfTo<DialogState>().AsSingle();
    }
}
