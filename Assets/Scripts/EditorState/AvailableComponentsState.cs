using System.Collections.Generic;
using Assets.Scripts.SceneState;
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
            public string name;

            /// <summary>
            /// The category of the component. It can also contain subcategories, separated by a '/' (forward slash)
            /// </summary>
            /// <example>Animation/Movement</example>
            public string category;

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
                name = "BoxShape",
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("BoxShape", "Shape")
            },
            new AvailableComponent
            {
                name = "SphereShape",
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape")
            },
            new AvailableComponent
            {
                name = "PlaneShape",
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("PlaneShape", "Shape")
            },
            new AvailableComponent
            {
                name = "CylinderShape",
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("CylinderShape", "Shape")
            },
            new AvailableComponent
            {
                name = "ConeShape",
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("ConeShape", "Shape")
            },
        };
    }
}
