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
        SceneAsset,
        ImageAsset
    }

    public Dictionary<DropZoneCategory, List<IDropZoneHandler>> dropZones = new Dictionary<DropZoneCategory, List<IDropZoneHandler>>();

    public void BeginDrag(DropZoneCategory category)
    {
        if (!dropZones.ContainsKey(category)) return;

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

    public void RegisterHandler(IDropZoneHandler dropHandler, IEnumerable<DropZoneCategory> dropZoneCategories)
    {
        // dropHandler can already be in dropZones. So, it needs to be removed first
        foreach (var dropZone in dropZones.Values)
        {
            dropZone.Remove(dropHandler);
        }

        // dropHandler should end up in only the correct dropZone categories
        foreach (var dropZoneCategory in dropZoneCategories)
        {
            if (!dropZones.ContainsKey(dropZoneCategory))
            {
                dropZones.Add(dropZoneCategory, new List<IDropZoneHandler>());
            }

            dropZones[dropZoneCategory].Add(dropHandler);
        }
    }
}

public interface IDropZoneHandler
{
    void Enable();
    void Disable();
}
