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
                        availableComponentsState.GetComponentDefinitionByCodeName("BoxShape")
                    }
                },
                new EntityPreset
                {
                    name = "Sphere Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("SphereShape")
                    }
                },
                new EntityPreset
                {
                    name = "Plane Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("PlaneShape")
                    }
                },
                new EntityPreset
                {
                    name = "Cylinder Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("CylinderShape")
                    }
                },
                new EntityPreset
                {
                    name = "Cone Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("ConeShape")
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
                        availableComponentsState.GetComponentDefinitionByCodeName("BoxRenderer"),
                        availableComponentsState.GetComponentDefinitionByCodeName("BoxCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Sphere Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("SphereRenderer"),
                        availableComponentsState.GetComponentDefinitionByCodeName("SphereCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Plane Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("PlaneRenderer"),
                        availableComponentsState.GetComponentDefinitionByCodeName("PlaneCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Cylinder Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("CylinderRenderer"),
                        availableComponentsState.GetComponentDefinitionByCodeName("CylinderCollider")
                    }
                },
                new EntityPreset
                {
                    name = "Cone Entity",
                    components = new List<DclComponent.ComponentDefinition>
                    {
                        availableComponentsState.GetComponentDefinitionByCodeName("ConeRenderer"),
                        availableComponentsState.GetComponentDefinitionByCodeName("ConeCollider")
                    }
                }
            };
        }
    }
}
