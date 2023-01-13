using System.Collections.Generic;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.EditorState
{
    public class EntityPresetState
    {
        public struct EntityPreset
        {
            public string name;
            public List<DclComponent.ComponentDefinition> components;
        }

        public IEnumerable<EntityPreset> allEntityPresets => buildInPresets;


        private readonly IReadOnlyList<EntityPreset> buildInPresets = new List<EntityPreset>
        {
            new EntityPreset
            {
                name = "Empty Entity",
                components = new List<DclComponent.ComponentDefinition>()
            },
            new EntityPreset
            {
                name = "Box Entity",
                components = new List<DclComponent.ComponentDefinition>
                {
                    new DclComponent.ComponentDefinition("BoxShape", "Shape")
                }
            },
            new EntityPreset
            {
                name = "Sphere Entity",
                components = new List<DclComponent.ComponentDefinition>
                {
                    new DclComponent.ComponentDefinition("SphereShape", "Shape")
                }
            },
            new EntityPreset
            {
                name = "Plane Entity",
                components = new List<DclComponent.ComponentDefinition>
                {
                    new DclComponent.ComponentDefinition("PlaneShape", "Shape")
                }
            },
            new EntityPreset
            {
                name = "Cylinder Entity",
                components = new List<DclComponent.ComponentDefinition>
                {
                    new DclComponent.ComponentDefinition("CylinderShape", "Shape")
                }
            },
            new EntityPreset
            {
                name = "Cone Entity",
                components = new List<DclComponent.ComponentDefinition>
                {
                    new DclComponent.ComponentDefinition("ConeShape", "Shape")
                }
            }
        };
    }
}
