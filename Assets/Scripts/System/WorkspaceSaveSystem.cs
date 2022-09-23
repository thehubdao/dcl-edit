using UnityEngine;
using DynamicPanels;
using System;
using System.IO;

namespace Assets.Scripts.System
{
    public class WorkspaceSaveSystem
    {
        public string saveFilePath => Application.dataPath + "/panel_layout.txt";

        public void Save(DynamicPanelsCanvas canvas)
        {
            byte[] data = PanelSerialization.SerializeCanvasToArray(canvas);
            string dataString = Convert.ToBase64String(data);
            File.WriteAllText(saveFilePath, dataString);
        }

        public void Load(DynamicPanelsCanvas canvas)
        {
            if(!File.Exists(saveFilePath))
            {
                Debug.Log("Couldn't find workspace layout save file");
                return;
            }

            string dataString = File.ReadAllText(saveFilePath);
            byte[] data = Convert.FromBase64String(dataString);
            PanelSerialization.DeserializeCanvasFromArray(canvas, data);
        }
    }
}
