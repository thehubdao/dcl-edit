using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


namespace Assets.Scripts.SceneState
{
    public class DclEntity
    {
        // Names
        /// <summary>
        /// In order to reduce confusion, "name" is not used and marked obsolete
        /// </summary>
        [Obsolete("you probably don't want to use this", true)]
        public string Name;

        /// <summary>
        /// The name, that is used in the TypeScript constructor for the Entity.
        /// </summary>
        /// Might contain spaces and special characters.
        /// Might not be Unique.
        public string ShownName
        {
            get => CustomName != "" ? CustomName : DefaultName;
            //set => customName = value;
        }

        /// <summary>
        /// The name, that is set by the User.
        /// </summary>
        /// Might contain spaces and special characters.
        /// Might not be Unique.
        /// Might be Empty.
        public string CustomName
        {
            get => _customName;
            set
            {
                _customName = value;
                //ReevaluateExposeStatus();
            }
        }

        [SerializeField]
        private string _customName = "";

        /// <summary>
        /// The name, that is used as default 
        /// </summary>
        /// Is always "Entity"
        private const string DefaultName = "Entity";

        public bool IsExposed { get; set; }

        public Guid Id { get; set; }

        [CanBeNull]
        public DclEntity Parent
        {
            get => Scene.GetEntityById(_parentId);
            set => _parentId = value?.Id ?? Guid.Empty;
        }

        public Guid ParentId => _parentId;

        private Guid _parentId = Guid.Empty;

        public IEnumerable<DclEntity> Children =>
            Scene.AllEntities
                .Select(pair => pair.Value)
                .Where(e => e.Parent == this);

        public DclScene Scene { get; set; }

        public List<DclComponent> Components { get; } = new List<DclComponent>();

        public void AddComponent(DclComponent component)
        {
            component.Entity = this;
            Components.Add(component);
        }

        public DclComponent GetComponentByName(string name)
        {
            return Components.Exists(c => c.NameInCode == name) ? // if component exists
                Components.Find(c => c.NameInCode == name) : // then return component
                null; // else return null
        }

        public DclComponent GetFirstComponentByName(params string[] names)
        {
            return Components.FirstOrNull(c => names.Contains(c.NameInCode));
        }

        // Specific Components
        // Transform
        public DclTransformComponent GetTransformComponent()
        {
            var component = GetComponentByName("Transform");

            if (component == null)
                return null;

            var dclTransformComponent = new DclTransformComponent(component);
            return dclTransformComponent.Validate() ? dclTransformComponent : null;
        }

        /**
         * Checks if at least one of the specified component names exists in this entity
         */
        public bool HasComponent(params string[] names)
        {
            return names.Any(name => Components.Exists(c => c.NameInCode == name));
        }
        public DclEntity(Guid id, string name = "", Guid parentId = default, bool isExposed = false)
        {
            Id = id;
            _customName = name;
            _parentId = parentId;
            IsExposed = isExposed;
        }
        public Guid GenerateSeededGuid(int seed)
        {
            var r = new System.Random(seed);
            var guid = new byte[16];
            r.NextBytes(guid);

            return new Guid(guid);
        }
        public DclEntity DeepCopy(DclScene sceneState)
        {
            DclEntity deepcopyEntity = new DclEntity(Id, CustomName, _parentId, true);

            deepcopyEntity.Id = Guid.NewGuid();

            foreach (var component in this.Components)
            {
                DclComponent newComponent = component.DeepCopy();
                newComponent.Entity = deepcopyEntity;
                deepcopyEntity.AddComponent(newComponent);
            }
            
            sceneState.AddEntity(deepcopyEntity);
            
            if (Children.ToList().Count > 0)
            {
                foreach (var child in Children.ToList())
                {
                    DclEntity newChild = child.DeepCopy(sceneState);
                    newChild.Parent = deepcopyEntity;
                }
            }
            return deepcopyEntity;
        }
    }
}