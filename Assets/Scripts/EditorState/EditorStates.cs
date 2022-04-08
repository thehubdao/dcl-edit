using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class EditorStates : MonoBehaviour
    {
        // Current instance
        public static EditorStates Instance;


        // States
        public CameraState CameraState;

        


        // Static references to the current states
        public static CameraState CurrentCameraState => Instance.CameraState;

    }
}
