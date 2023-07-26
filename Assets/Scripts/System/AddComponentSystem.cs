using System;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class AddComponentSystem
    {
        // Dependencies
        private CommandSystem commandSystem;

        [Inject]
        private void Construct(CommandSystem commandSystem)
        {
            this.commandSystem = commandSystem;
        }

        public List<DclComponent.ComponentDefinition> GetAvailableComponents()
        {
            return new List<DclComponent.ComponentDefinition>
            {
                new DclComponent.ComponentDefinition("BoxShape", "Shape", true),
                new DclComponent.ComponentDefinition("SphereShape", "Shape", true),
                new DclComponent.ComponentDefinition("PlaneShape", "Shape", true),
                new DclComponent.ComponentDefinition("CylinderShape", "Shape", true),
                new DclComponent.ComponentDefinition("ConeShape", "Shape", true),
            };
        }

        public bool CanComponentBeAdded(DclEntity entity, DclComponent.ComponentDefinition component)
        {
            return entity.GetComponentBySlot(component.NameOfSlot) == null;
        }

        public void AddComponent(Guid entityId, DclComponent.ComponentDefinition component)
        {
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateAddComponent(entityId, component));
        }
    }
}
