using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaveSystem : MonoBehaviour
{
    private const float SaveIntervalInMinutes = 1f;
    private static float SaveIntervalInSeconds => SaveIntervalInMinutes * 60;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
    }

    private UndoManager.UndoItem? _lastSavedAtUndoItemReference;

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SaveIntervalInSeconds);

            UndoManager.UndoItem? currentUndoItem = null;

            try
            {
                currentUndoItem = UndoManager.CurrentItem;
            }
            catch
            {
                // ignored
            }

            if ( !_lastSavedAtUndoItemReference.Equals(currentUndoItem))
            {
                Debug.Log("Auto Saving");
                
                SceneSaveSystem.Save();
                _lastSavedAtUndoItemReference = currentUndoItem;
            }
            else
            {
                Debug.Log("Not Auto Saving. No change detected");
            }
        }
    }
}
