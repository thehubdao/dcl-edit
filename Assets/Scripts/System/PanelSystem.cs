using Assets.Scripts.EditorState;
using Zenject;
using DynamicPanels;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class PanelSystem
    {
        private UnityState unityState;
        
        [Inject]
        public void Construct(UnityState unityState)
        {
            this.unityState = unityState;
        }

        public void ChangePanelTabTitle(RectTransform panel, string newTitle)
        {
            var tab = PanelUtils.GetAssociatedTab(panel);
            if (tab != null)
                tab.Label = newTitle;
        }
    }
}