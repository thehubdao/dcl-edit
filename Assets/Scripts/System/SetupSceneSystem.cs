using System.Security.Policy;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class SetupSceneSystem : MonoBehaviour
    {
        public static void SetupScene(DclScene scene)
        {
            EditorStates.Instance.NewSceneState(scene);

            CameraSystem.CameraStartup();

            SetupSceneEventListenersSystem.SetupSceneEventListeners();
        }
    }
}
