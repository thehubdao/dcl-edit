using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveComponent : MonoBehaviour
{
    [NonSerialized]
    public EntityComponent component;

    public void Remove()
    {
        Destroy(component);
    }
}
