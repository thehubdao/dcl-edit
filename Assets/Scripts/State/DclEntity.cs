using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.State
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


        public Guid Id { get; set; }

        [CanBeNull]
        public DclEntity Parent { get; set; }

        public DclScene Scene { get; }

        public List<DclComponent> Components { get; } = new List<DclComponent>();
        

        public DclEntity(DclScene scene, Guid id, string name = "", DclEntity parent = null)
        {
            Scene = scene;
            Id = id;
            _customName = name;
            Parent = parent;

            scene.AllEntities.Add(id,this);
        }
    }
}

