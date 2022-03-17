using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveBackupSystem : MonoBehaviour
{
    private static UndoManager.UndoItem? _lastSavedAtUndoItemReference;

    public static void BackupSave()
    {
        UndoManager.UndoItem? currentUndoItem = null;
        currentUndoItem = UndoManager.CurrentItem;
        if (!_lastSavedAtUndoItemReference.Equals(currentUndoItem))
        {
            Directory.CreateDirectory(DclSceneManager.DclProjectPath + "/dcl-edit/backups");
            var savesPath = DclSceneManager.DclProjectPath + "/dcl-edit/saves";

            if (Directory.Exists(savesPath))
            {
                var backupPath = "";
                var i = 0;
                do
                {
                    i++;
                    var now = DateTime.Now;
                    var backupName = $"backup_{now.Year}-{now.Month}-{now.Day}_{now.Hour}-{now.Minute}-{now.Second}" + (i > 1 ? $"_{i}" : "");

                    backupPath = DclSceneManager.DclProjectPath + "/dcl-edit/backups/" + backupName;
                } while (Directory.Exists(backupPath));

                if (PersistentData.NumberOfBackups != 0)
                {
                    Directory.CreateDirectory(backupPath);

                    CopyFilesRecursively(savesPath, backupPath);
                }
                NumberOfBackups(backupPath);

                }
        _lastSavedAtUndoItemReference = currentUndoItem;
        }
    }
    private static void NumberOfBackups(string backupPath)
    {

        int number = PersistentData.NumberOfBackups;
        if (number > 0)
        {
            string[] backups = Directory.GetDirectories(DclSceneManager.DclProjectPath + "/dcl-edit/backups/");
            if (backups.Length > number)
            {
                Array.Sort(backups);//Sorting by name ought to yield oldest first
                string oldestBackupDirectory = backups[0];
                backups = Directory.GetFiles(oldestBackupDirectory);

                foreach (string oldAddress in backups)
                {
                    File.Delete(oldAddress);//delete files first 
                }
                Directory.Delete(oldestBackupDirectory);//then delete directory 
            }
        }
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}
