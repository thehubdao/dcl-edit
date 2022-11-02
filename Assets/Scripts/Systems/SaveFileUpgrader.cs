using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFileUpgrader : MonoBehaviour,ISerializedFieldToStatic
{
    [SerializeField]
    private GameObject newSceneWindow;

    private static GameObject _newSceneWindow;

    public void SetupStatics()
    {
        _newSceneWindow = newSceneWindow;
    }

    
    private static readonly int currentSaveFileVersion = 2;

    private class JsonWrapper
    {
        public JsonWrapper()
        {
            version = currentSaveFileVersion;
        }
        public int version;
    }

    public static void CheckSaveFiles()
    {
        var saveVersionPath = DclSceneManager.DclProjectPath + "/dcl-edit/saves/version.json";
        var projectSaveVersion = 0;

        if (File.Exists(saveVersionPath))
        {
            projectSaveVersion = JsonUtility.FromJson<JsonWrapper>(File.ReadAllText(saveVersionPath)).version;
        }
        else
        {
            _newSceneWindow.SetActive(true);
        }

        UpgradeSaveFiles(projectSaveVersion);
    }

    private static void UpgradeSaveFiles(int fromVersion)
    {
        if (fromVersion < currentSaveFileVersion)
        {
            var newVersion = fromVersion switch
            {
                0 => UpgradeFrom(0),
                1 => UpgradeFrom(1),
                _ => throw new ArgumentOutOfRangeException(nameof(fromVersion), fromVersion, "version not implemented yet"),
            };
            UpgradeSaveFiles(newVersion);
        }
    } 
    
    private static int UpgradeFrom(int version)
    {
        Debug.Log($"Upgrading Save file from version {version} to {version + 1}");

        var savesDirectoryPath = DclSceneManager.DclProjectPath + "/dcl-edit/saves/";
        var saveVersionPath = savesDirectoryPath + "version.json";

        Directory.CreateDirectory(savesDirectoryPath);
        
        File.WriteAllText(saveVersionPath,JsonUtility.ToJson(new JsonWrapper()));

        return version + 1;
    }
}
