using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void Save()
    {
        SaveBackupSystem.BackupSave();
        SceneSaveSystem.Save(); 
        ScriptGenerator.MakeScript();
        AssetSaverSystem.Save();
    }
}
