using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Visuals;
using UnityEngine;
using Zenject;

public class EntityVisualsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindFactory<GltfShapeVisuals, GltfShapeVisuals.Factory>().FromNewComponentOn(gameObject);
    }
}
