using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.State
{
    public class DclEntity : MonoBehaviour
    {
        [CanBeNull] 
        private DclEntity _parent;

        [CanBeNull]
        public DclEntity Parent
        {
            get => _parent;
            set => _parent = value;
        }

        private DclScene _scene;

        public DclScene Scene
        {
            get => _scene;
            set => _scene = value;
        }

        private DclComponent[] Components => GetComponents<DclComponent>();
    }
}

