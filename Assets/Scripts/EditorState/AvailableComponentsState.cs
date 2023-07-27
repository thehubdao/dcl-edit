using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class AvailableComponentsState
    {
        public struct AvailableComponent
        {
            /// <summary>
            /// The display name of the component
            /// </summary>
            /// <example>MoveUp</example>
            public string name => componentDefinition.NameInCode;

            /// <summary>
            /// The category of the component. It can also contain subcategories, separated by a '/' (forward slash)
            /// </summary>
            /// <example>Animation/Movement</example>
            public string category;

            /// <summary>
            /// Whether or not this component is listed in the Add Component menu at the bottom of the inspector
            /// </summary>
            public bool availableInAddComponentMenu;

            /// <summary>
            /// The ComponentDefinition of the component
            /// </summary>
            public DclComponent.ComponentDefinition componentDefinition;

            public override bool Equals(object obj)
            {
                return obj is AvailableComponent other &&
                       category.Equals(other.category) &&
                       availableInAddComponentMenu.Equals(other.availableInAddComponentMenu) &&
                       componentDefinition.Equals(other.componentDefinition);
            }

            public override int GetHashCode()
            {
                return (category, availableInAddComponentMenu, componentDefinition).GetHashCode();
            }
        }

        public IEnumerable<AvailableComponent> allAvailableComponents => buildInComponents.Concat(customComponents);


        private readonly IReadOnlyList<AvailableComponent> buildInComponents = new List<AvailableComponent>
        {
            new AvailableComponent
            {
                category = "Built-in",
                availableInAddComponentMenu = false,
                componentDefinition = DclTransformComponent.transformComponentDefinition
            },
            new AvailableComponent
            {
                category = "Built-in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("BoxShape", "Shape", true)
            },
            new AvailableComponent
            {
                category = "Built-in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape", true)
            },
            new AvailableComponent
            {
                category = "Built-in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("PlaneShape", "Shape", true)
            },
            new AvailableComponent
            {
                category = "Built-in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("CylinderShape", "Shape", true)
            },
            new AvailableComponent
            {
                category = "Built-in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("ConeShape", "Shape", true)
            },
            new AvailableComponent
            {
                category = "Built-in/Shape",
                availableInAddComponentMenu = false,
                componentDefinition = DclGltfShapeComponent.gltfShapeComponentDefinition
            },
            new AvailableComponent
            {
                category = "Built-in",
                availableInAddComponentMenu = true,
                componentDefinition = DclSceneComponent.sceneComponentDefinition
            }
        };

        private readonly List<AvailableComponent> customComponents = new List<AvailableComponent>();

        public DclComponent.ComponentDefinition GetComponentDefinitionByName(string name)
        {
            try
            {
                return allAvailableComponents.First(c => c.name == name).componentDefinition;
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException($"No component definition for the name {name} found");
            }
        }

        public void UpdateCustomComponent(AvailableComponent newComponent)
        {
            // see if component already exists
            AvailableComponent? existingCustomComponent = null;
            foreach (var c in customComponents.Where(c => c.name == newComponent.name))
            {
                existingCustomComponent = c;
                break;
            }

            // delete existing
            if (existingCustomComponent.HasValue)
            {
                customComponents.Remove(existingCustomComponent.Value);
            }

            customComponents.Add(newComponent);
        }
    }
}
