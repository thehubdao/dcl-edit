using Assets.Scripts.Interaction;
using UnityEngine;
using Zenject;

public class EntityInstaller : MonoInstaller
{
    [SerializeField]
    private GameObject _entityVisualPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<EntitySelectInteraction, EntitySelectInteraction.Factory>().FromComponentInNewPrefab(_entityVisualPrefab);
    }
}