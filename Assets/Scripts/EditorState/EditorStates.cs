using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class EditorStates : MonoBehaviour
    {
        // Current instance
        public static EditorStates Instance;


        // States
        // Camera State
        [SerializeField]
        public CameraState CameraState;

        // Input State
        [SerializeField]
        public InputState InputState;

        // Scene State
        [SerializeField]
        public SceneState SceneState;

        // Project State
        [SerializeField]
        public ProjectState ProjectState;

        // Path State 
        [SerializeField]
        public PathState PathState;

        // Unity State
        [SerializeField]
        public UnityState UnityState;

        // Model Cache State
        [SerializeField]
        public ModelCacheState ModelCacheState;

        // Interface 3D State
        [SerializeField]
        public Interface3DState Interface3DState;


        // Static references to the current states
        public static CameraState CurrentCameraState => Instance.CameraState;
        public static InputState CurrentInputState => Instance.InputState;
        public static SceneState CurrentSceneState => Instance.SceneState;
        public static ProjectState CurrentProjectState => Instance.ProjectState;
        public static PathState CurrentPathState => Instance.PathState;
        public static UnityState CurrentUnityState => Instance.UnityState;
        public static ModelCacheState CurrentModelCacheState => Instance.ModelCacheState;
        public static Interface3DState CurrentInterface3DState => Instance.Interface3DState;
    }
}
