using System;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class AddComponentSystem
    {
        public List<DclComponent.ComponentDefinition> GetAvailableComponents()
        {
            return new List<DclComponent.ComponentDefinition>
            {
                new DclComponent.ComponentDefinition("BoxShape", "Shape"),
                new DclComponent.ComponentDefinition("SphereShape", "Shape"),
                new DclComponent.ComponentDefinition("PlaneShape", "Shape"),
                new DclComponent.ComponentDefinition("CylinderShape", "Shape"),
                new DclComponent.ComponentDefinition("ConeShape", "Shape"),
            };
        }

        public void AddComponent(Guid entityId, DclComponent.ComponentDefinition component)
        {
            Debug.Log($"Adding {component.NameInCode} component to {entityId.Shortened()}");
        }
    }
}
