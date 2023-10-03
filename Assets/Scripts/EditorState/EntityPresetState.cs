using System.Collections.Generic;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class EntityPresetState
    {
        public struct EntityPreset
        {
            public string name;
            public List<DclComponent.ComponentDefinition> components;
        }

        // Dependencies
        private AvailableComponentsState availableComponentsState;

        [Inject]
        private void Construct(AvailableComponentsState availableComponentsState)
        {
            this.availableComponentsState = availableComponentsState;
        }


        public IEnumerable<EntityPreset> allEntityPresets => buildInPresets;


        private IReadOnlyList<EntityPreset> buildInPresets;

        public void FillEcs6BuildInPresets()
        {
            buildInPresets = new List<EntityPreset>
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
                        availableComponentsState.GetComponentDefinitionByName("BoxShape")
                    }
                },
                new EntityPreset
                {
                    name = "Sphere Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("SphereShape")
                    }
                },
                new EntityPreset
                {
                    name = "Plane Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("PlaneShape")
                    }
                },
                new EntityPreset
                {
                    name = "Cylinder Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {   
                        availableComponentsState.GetComponentDefinitionByName("CylinderShape")
                    }
                },
                new EntityPreset
                {
                    name = "Cone Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("ConeShape")
                    }
                }
            };
        }

        public void FillEcs7BuildInPresets()
        {
            buildInPresets = new List<EntityPreset>
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
                        availableComponentsState.GetComponentDefinitionByName("BoxRenderer"),
                        availableComponentsState.GetComponentDefinitionByName("BoxCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Sphere Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("SphereRenderer"),
                        availableComponentsState.GetComponentDefinitionByName("SphereCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Plane Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("PlaneRenderer"),
                        availableComponentsState.GetComponentDefinitionByName("PlaneCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Cylinder Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("CylinderRenderer"),
                        availableComponentsState.GetComponentDefinitionByName("CylinderCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Cone Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByName("ConeRenderer"),
                        availableComponentsState.GetComponentDefinitionByName("ConeCollider")
                    }
                }
            };
        }
    }
}
