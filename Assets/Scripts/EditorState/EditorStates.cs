using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class EditorStates : MonoBehaviour
    {
        // Current instance
        public static EditorStates Instance;


        // States
        [SerializeField]
        public CameraState CameraState;
        
        [SerializeField]
        public InputState InputState;



        // Static references to the current states
        public static CameraState CurrentCameraState => Instance.CameraState;
        public static InputState CurrentInputState => Instance.InputState;

    }
}
