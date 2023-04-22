using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;

namespace Visuals.UiHandler
{
    public class SpacerHandler: MonoBehaviour
    {
        [SerializeField]
        public ClickHandler clickHandler;

        [SerializeField]
        public DropHandler dropHandler;
    }
}
