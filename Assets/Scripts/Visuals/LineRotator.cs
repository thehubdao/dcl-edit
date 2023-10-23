using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{

    public class LineRotator : MonoBehaviour
    {
        public enum RelevantAxis{
            X,
            Y,
            Z
        }

        public RelevantAxis axis;
        
        // Dependencies
        private CameraState cameraState;

        [Inject]
        public void Construct(CameraState cameraState)
        {
            this.cameraState = cameraState;
        }

        private void Update()
        {
            var relativeCamPos = transform.parent.InverseTransformPoint(cameraState.Position);

            if (axis == RelevantAxis.X)
            {
                if (relativeCamPos.y > 0 && relativeCamPos.z > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (relativeCamPos.y < 0 && relativeCamPos.z > 0)
                {
                    transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                else if (relativeCamPos.y > 0 && relativeCamPos.z < 0)
                {
                    transform.localRotation = Quaternion.Euler(270, 0, 0);
                }
                else if (relativeCamPos.y < 0 && relativeCamPos.z < 0)
                {
                    transform.localRotation = Quaternion.Euler(180, 0, 0);
                }
            } else if (axis == RelevantAxis.Y)
            {
                if (relativeCamPos.x > 0 && relativeCamPos.z > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (relativeCamPos.x < 0 && relativeCamPos.z > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                else if (relativeCamPos.x < 0 && relativeCamPos.z < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else if (relativeCamPos.x > 0 && relativeCamPos.z < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            } else if (axis == RelevantAxis.Z)
            {
                if (relativeCamPos.x > 0 && relativeCamPos.y > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (relativeCamPos.x < 0 && relativeCamPos.y > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                else if (relativeCamPos.x < 0 && relativeCamPos.y < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 180);
                }
                else if (relativeCamPos.x > 0 && relativeCamPos.y < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 270);
                }
            }

            
        }
    }
}
