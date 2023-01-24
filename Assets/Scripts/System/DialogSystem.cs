using Assets.Scripts.Events;
using System;
using Zenject;

public class DialogSystem
{
    // Dependencies
    private EditorEvents editorEvents;

    [Inject]
    void Construct(EditorEvents editorEvents)
    {
        this.editorEvents = editorEvents;
    }

    /// <summary>
    /// Lets the user select an asset which will then be added to a GLTFShapeComponent on the entity with the targetEntityId.
    /// </summary>
    /// <param name="targetEntityId"></param>
    public void OpenAssetDialog(Guid targetEntityId)
    {
        UnityEngine.Debug.Log("Open Asset Dialog");
    }
}
