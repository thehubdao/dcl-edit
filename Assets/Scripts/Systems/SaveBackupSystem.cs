using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveBackupSystem : MonoBehaviour
{
    public static void BackupSave()
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
    }
    private static void NumberOfBackups(string backupPath)
    {

        int number = PersistentData.NumberOfBackups;
        if (number < 0)
        {
            //Unlimited Backups
        }
        else if (number == 0)
        {
            //No Backups
            //Should Prevent Saving
        }
        else
        {
            string[] backups = Directory.GetDirectories(DclSceneManager.DclProjectPath + "/dcl-edit/backups/");
            if (backups.Length > number)
            {
                Array.Sort(backups);//Sorting by name ought to yield oldest first
                string oldestBackupDirectory = backups[0];
                backups = Directory.GetFiles(oldestBackupDirectory);

                foreach (string oldAdress in backups)
                {
                    File.Delete(oldAdress);//delete files first 
                }
                Directory.Delete(oldestBackupDirectory);//then delelete directory 
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
