using Assets.Scripts.SceneState;
using System.Collections.Generic;
using System.Linq;

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
            /// The ComponentDefinition of the component
            /// </summary>
            public DclComponent.ComponentDefinition componentDefinition;
        }

        public IEnumerable<AvailableComponent> allAvailableComponents => buildInComponents;


        private readonly IReadOnlyList<AvailableComponent> buildInComponents = new List<AvailableComponent>
        {
            new AvailableComponent
            {
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("BoxShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("PlaneShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("CylinderShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in/Shape",
                componentDefinition = new DclComponent.ComponentDefinition("ConeShape", "Shape")
            },
            new AvailableComponent
            {
                category = "Build in",
                componentDefinition = DclSceneComponent.sceneComponentDefinition
            }
        };

        public DclComponent.ComponentDefinition GetComponentDefinitionByName(string name)
        {
            return allAvailableComponents.First(c => c.name == name).componentDefinition;
        }
    }
}
