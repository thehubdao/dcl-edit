using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;


namespace Assets.Scripts.EditorState
{
    public class CameraState : MonoBehaviour
    {
        private float CameraNormalFlySpeed => PersistentData.CameraSpeed;
        private float CameraFastFlySpeed => CameraNormalFlySpeed * 3;
        private float MouseSensitivity => PersistentData.MouseSensitivity;


        private Vector3 _position;
        private float _pitch;
        private float _yaw;

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                OnCameraStateChanged.Invoke();
            }
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;

                if (_pitch > 100) // Yes, 100 degrees is correct. In my opinion, it feels less restrictive when looking straight up or down.
                    _pitch = 100;

                if (_pitch < -100)
                    _pitch = -100;

                OnCameraStateChanged.Invoke();
            }
        }

        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                OnCameraStateChanged.Invoke();
            }
        }

        public Quaternion Rotation => Quaternion.Euler(_pitch, _yaw, 0);
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Back => Rotation * Vector3.back;
        public Vector3 Right => Rotation * Vector3.right;
        public Vector3 Left => Rotation * Vector3.left;
        public Vector3 Up => Rotation * Vector3.up;
        public Vector3 Down => Rotation * Vector3.down;
 
        public void RotateStep(Vector2 direction)
        {
            RotateStep(direction.x, direction.y);
        }

        public void RotateStep(float directionX, float directionY)
        {
            Yaw += directionX * MouseSensitivity;
            Pitch += directionY * -MouseSensitivity;
        }

        public void MoveContinuously(Vector3 localDirection, bool isFast)
        {
            MoveStep(localDirection * Time.deltaTime, isFast);
        }

        public void MoveStep(Vector3 localDirection, bool isFast)
        {
            Position += Rotation * localDirection * (isFast ? CameraFastFlySpeed : CameraNormalFlySpeed);
        }
        public void MoveFixed(Vector3 localDirection)
        {
            Position += Rotation * localDirection;
        }

        public void RotateAroundPointStep(Vector3 point, Vector2 direction)
        {
            // Find relations from position to Pivot point
            var gimbalVector = Position - point;
            var moveToPivot = Quaternion.Inverse(Rotation) * -gimbalVector;

            // Move to pivot
            MoveFixed(moveToPivot);

            // Rotate
            Yaw += direction.x * MouseSensitivity;
            Pitch += direction.y * -MouseSensitivity;

            // Move back from pivot
            MoveFixed(-moveToPivot);
        }


        public UnityEvent OnCameraStateChanged = new UnityEvent();
    }
}
