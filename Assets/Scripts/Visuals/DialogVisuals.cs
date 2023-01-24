using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

public class DialogVisuals : MonoBehaviour
{
    // Dependencies
    UnityState unityState;

    [Inject]
    void Construct(UnityState unityState)
    {
        this.unityState = unityState;
    }
}
