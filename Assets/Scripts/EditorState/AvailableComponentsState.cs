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
        }

        public IEnumerable<AvailableComponent> allAvailableComponents => buildInComponents;


        private readonly IReadOnlyList<AvailableComponent> buildInComponents = new List<AvailableComponent>
        {
            new AvailableComponent
            {
                category = "Build in",
                availableInAddComponentMenu = false,
                componentDefinition = DclTransformComponent.transformComponentDefinition
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("BoxShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("PlaneShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("CylinderShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition("ConeShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                availableInAddComponentMenu = false,
                componentDefinition = DclGltfShapeComponent.gltfShapeComponentDefinition
            },
            new AvailableComponent
            {
                category = "Build in",
                availableInAddComponentMenu = true,
                componentDefinition = DclSceneComponent.sceneComponentDefinition
            }
        };

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
    }
}
