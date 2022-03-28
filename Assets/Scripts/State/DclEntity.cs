using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.State
{
    public class DclEntity : MonoBehaviour
    {
        // Names

        [Obsolete("you probably don't want to use this", true)]
        [NonSerialized]
        public new string name;

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


        
        [SerializeField] 
        private Guid _id;

        public Guid Id
        {
            get => _id;
            set => _id = value;
        }
        
        [SerializeField]
        [CanBeNull] 
        private DclEntity _parent;

        [CanBeNull]
        public DclEntity Parent
        {
            get => _parent;
            set => _parent = value;
        }

        [SerializeField]
        private DclScene _scene;

        public DclScene Scene
        {
            get => _scene;
            set => _scene = value;
        }
        
        private DclComponent[] Components => GetComponents<DclComponent>();


        [Header("Editor Setup")] 
        [SerializeField]
        private GameObject _childrenGameObject;

        public GameObject ChildrenGameObject
        {
            get => _childrenGameObject;
            set => _childrenGameObject = value;
        }

        [SerializeField]
        private GameObject _componentsGameObject;

        public GameObject ComponentsGameObject
        {
            get => _componentsGameObject;
            set => _componentsGameObject = value;
        }

    }
}

