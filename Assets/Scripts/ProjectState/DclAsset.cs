using UnityEngine;

namespace Assets.Scripts.ProjectState
{
    public abstract class DclAsset
    {
        public abstract string Name { get; }

        public virtual Texture2D Thumbnail => null; // Todo: add default thumbnail

        public abstract string TypeName { get; }
    }
}
