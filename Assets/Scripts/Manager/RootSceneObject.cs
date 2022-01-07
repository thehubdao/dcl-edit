using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSceneObject : SceneTreeObject
{
    public override SceneTreeObject Parent
    {
        get => null;
        set => throw new System.NotImplementedException();
    }
}
