using UnityEngine;
using DynamicPanels;
using System;
using Zenject;

namespace Assets.Scripts.System
{

    public class WorkspaceSaveSystem
    {
        //Dependency
        private SettingsSystem settingSystem;

        [Inject]
        public void Construct(
            SettingsSystem settingSystem)
        {
            this.settingSystem = settingSystem;
        }

        public void Save(DynamicPanelsCanvas canvas)
        {
            byte[] data = PanelSerialization.SerializeCanvasToArray(canvas);
            string dataString = Convert.ToBase64String(data);
            settingSystem.panelSize.Set(dataString);
        }

        public void Load(DynamicPanelsCanvas canvas)
        {
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
