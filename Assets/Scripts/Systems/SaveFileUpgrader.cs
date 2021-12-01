using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFileUpgrader : MonoBehaviour
{
    private static readonly int currentSaveFileVersion = 1;

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
        var saveVersionPath = SceneManager.DclProjectPath + "/dcl-edit/saves/version.json";
        var projectSaveVersion = 0;

        if (File.Exists(saveVersionPath))
        {
            projectSaveVersion = JsonUtility.FromJson<JsonWrapper>(File.ReadAllText(saveVersionPath)).version;
        }

        UpgradeSaveFiles(projectSaveVersion);
    }

    private static void UpgradeSaveFiles(int fromVersion)
    {
        if (fromVersion < currentSaveFileVersion)
        {
            var newVersion = fromVersion switch
            {
                0 => UpgradeFrom0(),
                _ => throw new ArgumentOutOfRangeException(nameof(fromVersion), fromVersion, "version not implemented yet"),
            };
            UpgradeSaveFiles(newVersion);
        }
    }

    private static int UpgradeFrom0()
    {
        Debug.Log("Upgrading Save file from version 0 to 1");
        
        var saveVersionPath = SceneManager.DclProjectPath + "/dcl-edit/saves/version.json";
        
        File.WriteAllText(saveVersionPath,JsonUtility.ToJson(new JsonWrapper()));

        return 1;
    }

}
