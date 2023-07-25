using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using Zenject;

namespace Visuals.UiHandler
{
    public class SpacerHandler: MonoBehaviour
    {
        [SerializeField]
        public ClickHandler clickHandler;

        [SerializeField]
        public DropHandler dropHandler;

        public class Factory : PlaceholderFactory<SpacerHandler>
        {
        }
    }
}
