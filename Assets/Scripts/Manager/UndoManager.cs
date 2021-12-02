using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    private static List<UndoItem> undoItems = new List<UndoItem>();
    private static int currentIndex = -1;

    public static bool CanUndo() => !(currentIndex < 0);
    public static bool CanRedo() => currentIndex < undoItems.Count;

    public static void Undo()
    {
        if (!CanUndo())
            return;

        undoItems[currentIndex].UndoAction.Invoke();
        currentIndex--;
    }

    public static void Redo()
    {
        if (!CanRedo())
            return;

        currentIndex++;
        undoItems[currentIndex].RedoAction.Invoke();
    }

    public static void RecordUndoItem(string name, Action undo, Action redo)
    {
        // remove all following items to rerecord Undo list
        undoItems.RemoveRange(currentIndex + 1, undoItems.Count - (currentIndex + 1));

        undoItems.Add(new UndoItem(name, undo, redo));
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
}


