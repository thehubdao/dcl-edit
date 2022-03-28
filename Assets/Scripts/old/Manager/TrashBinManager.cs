using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBinManager : Manager, ISerializedFieldToStatic
{
    [SerializeField]
    private Transform _entitiesParent;
    public static Transform EntitiesParent { get; private set; }

    [SerializeField]
    private Transform _trashParent;
    public static Transform TrashParent { get; private set; }


    public static void DeleteEntity(Entity e)
    {
        e.transform.parent = TrashParent;
    }

    public static void RestoreEntity(Entity e)
    {
        e.transform.parent = EntitiesParent;
    }

    public void SetupStatics()
    {
        EntitiesParent = _entitiesParent;
        TrashParent = _trashParent;
    }
}
