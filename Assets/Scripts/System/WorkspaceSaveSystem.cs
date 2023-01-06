using UnityEngine;
using DynamicPanels;
using System;
using Zenject;
using Assets.Scripts.EditorState;

namespace Assets.Scripts.System
{
    public class WorkspaceSaveSystem
    {
        //Dependency
        private SettingsSystem settingSystem;
        private UnityState unityState;

        [Inject]
        public void Construct(
            SettingsSystem settingSystem,
            UnityState unityState)
        {
            this.settingSystem = settingSystem;
            this.unityState = unityState;
        }

        public void Save()
        {
            DynamicPanelsCanvas canvas = unityState.dynamicPanelsCanvas;
            byte[] data = PanelSerialization.SerializeCanvasToArray(canvas);
            string dataString = Convert.ToBase64String(data);
            settingSystem.panelSize.Set(dataString);
        }

        public void Load()
        {
            DynamicPanelsCanvas canvas = unityState.dynamicPanelsCanvas;

            if (settingSystem.panelSize.Get() == "")
            {
                Debug.Log("Layout save is empty");
                return;
            }

            byte[] data = Convert.FromBase64String(settingSystem.panelSize.Get());
            PanelSerialization.DeserializeCanvasFromArray(canvas, data);
        }
    }
}
