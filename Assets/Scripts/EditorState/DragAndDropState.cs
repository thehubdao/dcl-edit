using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropState
{
    public enum DropZoneCategory
    {
        Entity,
        ModelAsset,
    }

    public Dictionary<DropZoneCategory, List<IDropZoneHandler>> dropZones = new Dictionary<DropZoneCategory, List<IDropZoneHandler>>();

    public void BeginDrag(DropZoneCategory category)
    {
        foreach (var dropZoneHandler in dropZones[category])
        {
            dropZoneHandler.Enable();
        }
    }

    public void EndDrag()
    {
        foreach (var dropZoneHandler in dropZones.Values.Flatten())
        {
            dropZoneHandler.Disable();
        }
    }

    public void RegisterHandler(IDropZoneHandler dropHandler, DropZoneCategory? dropZoneCategory)
    {
        // dropHandler can already be in dropZones. So, it needs to be removed first
        foreach (var dropZone in dropZones.Values)
        {
            dropZone.Remove(dropHandler);
        }

        // when the category is null, don't add it anywhere
        if (!dropZoneCategory.HasValue) return;

        // dropHandler should end up in only the correct dropZone category
        if (!dropZones.ContainsKey(dropZoneCategory.Value))
        {
            dropZones.Add(dropZoneCategory.Value, new List<IDropZoneHandler>());
        }

        dropZones[dropZoneCategory.Value].Add(dropHandler);
    }
}

public interface IDropZoneHandler
{
    void Enable();
    void Disable();
}
