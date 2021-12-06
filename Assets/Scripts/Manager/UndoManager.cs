using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UndoManager : MonoBehaviour
{
    private static List<UndoItem> undoItems = new List<UndoItem>();
    private static int currentIndex = -1;
    public static UnityEvent onUpdate = new UnityEvent();

    public static bool CanUndo() => !(currentIndex < 0);
    public static bool CanRedo() => currentIndex < undoItems.Count-1;

    public static void Undo()
    {
        //Debug.Log($"Try Undoing");

        if (!CanUndo())
            return;
        
        //Debug.Log($"Undoing {undoItems[currentIndex].Name} at ID {currentIndex}");
        
        undoItems[currentIndex].UndoAction.Invoke();
        currentIndex--;

        onUpdate.Invoke();
    }

    public static void Redo()
    {
        if (!CanRedo())
            return;
        
        currentIndex++;
        undoItems[currentIndex].RedoAction.Invoke();
        
        //Debug.Log($"Redoing {undoItems[currentIndex].Name} at ID {currentIndex}");

        onUpdate.Invoke();
    }

    public static void RecordUndoItem(string name, Action undo, Action redo)
    {
        //Debug.Log("Add undo item "+name);
        

        // remove all following items to rerecord Undo list
        undoItems.RemoveRange(currentIndex + 1, undoItems.Count - (currentIndex + 1));

        undoItems.Add(new UndoItem(name, undo, redo));
        currentIndex = undoItems.Count - 1;

        onUpdate.Invoke();
    }

    public struct UndoItem
    {
        public string Name { get; }
        public Action UndoAction { get; }
        public Action RedoAction { get; }

        public UndoItem(string name, Action undo, Action redo)
        {
            Name = name;
            UndoAction = undo;
            RedoAction = redo;
        }
    }


    internal IEnumerable<string> GetAllUndoItemNames => undoItems.Select(item => item.Name);
    internal int GetCurrentIndex => currentIndex;

}

#if UNITY_EDITOR

[CustomEditor(typeof(UndoManager))]
public class LookAtPointEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        if (target is UndoManager undoManager)
        {
            var i = 0;
            foreach (var itemName in undoManager.GetAllUndoItemNames)
            {
                GUILayout.Label((i == undoManager.GetCurrentIndex ? "* " : "  ")+itemName);
                i++;
            }
        }
    }
}

#endif
