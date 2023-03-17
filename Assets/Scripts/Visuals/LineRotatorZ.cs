using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{

    public class LineRotatorZ : MonoBehaviour
    {
        // Dependencies
        private CameraState cameraState;

        private Quaternion ZeroRotation = Quaternion.Euler(0, 0, 0);
        private Quaternion NinetyRotation = Quaternion.Euler(0, 0, 90);
        private Quaternion OneEightyRotation = Quaternion.Euler(0, 0, 180);
        private Quaternion TwoSeventyRotation = Quaternion.Euler(0, 0, 270);

        [Inject]
        public void Construct(CameraState cameraState)
        {
            this.cameraState = cameraState;
        }

        private void Update()
        {
            var relativeCamPos = transform.parent.InverseTransformPoint(cameraState.Position);

            Debug.Log(relativeCamPos);

            if (relativeCamPos.x > 0 && relativeCamPos.y > 0 && relativeCamPos.z > 0)
            {
                transform.rotation = ZeroRotation;
            }
            else if (relativeCamPos.x > 0 && relativeCamPos.y > 0 && relativeCamPos.z < 0)
            {
                transform.rotation = ZeroRotation;
            }
            else if (relativeCamPos.x < 0 && relativeCamPos.y > 0 && relativeCamPos.z > 0)
            {
                transform.rotation = NinetyRotation;
            }
            else if (relativeCamPos.x < 0 && relativeCamPos.y > 0 && relativeCamPos.z < 0)
            {
                transform.rotation = NinetyRotation;
            }
            else if (relativeCamPos.x < 0 && relativeCamPos.y < 0 && relativeCamPos.z > 0)
            {
                transform.rotation = OneEightyRotation;
            }
            else if (relativeCamPos.x < 0 && relativeCamPos.y < 0 && relativeCamPos.z < 0)
            {
                transform.rotation = OneEightyRotation;
            }
            else if (relativeCamPos.x > 0 && relativeCamPos.y < 0 && relativeCamPos.z < 0)
            {
                transform.rotation = TwoSeventyRotation;
            }
            else if (relativeCamPos.x > 0 && relativeCamPos.y < 0 && relativeCamPos.z > 0)
            {
                transform.rotation = TwoSeventyRotation;
            }
        }
    }
}
