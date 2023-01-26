using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using UnityEngine;


namespace Assets.Scripts.EditorState
{
    public class CameraState
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
                _editorEvents.InvokeCameraStateChangedEvent();
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

                _editorEvents.InvokeCameraStateChangedEvent();
            }
        }

        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                _editorEvents.InvokeCameraStateChangedEvent();
            }
        }

        public Quaternion Rotation => Quaternion.Euler(_pitch, _yaw, 0);
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Back => Rotation * Vector3.back;
        public Vector3 Right => Rotation * Vector3.right;
        public Vector3 Left => Rotation * Vector3.left;
        public Vector3 Up => Rotation * Vector3.up;
        public Vector3 Down => Rotation * Vector3.down;


        // Dependencies
        private EditorEvents _editorEvents;

        public CameraState(EditorEvents editorEvents)
        {
            _editorEvents = editorEvents;
        }


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
        /// <summary>
        /// Moves the camera towards the given destination. Returns true if the destination was reached.
        /// </summary>
        public bool MoveTowards(Vector3 globalDestination, bool isFast)
        {
            Vector3 dirToDest = globalDestination - Position;
            Vector3 move = dirToDest.normalized * Time.deltaTime * (isFast ? CameraFastFlySpeed : CameraNormalFlySpeed);

            // Check if destination was reached
            if (dirToDest.magnitude < move.magnitude)
            {
                Position += dirToDest;
                return true;
            }

            MoveContinuously(Quaternion.Inverse(Rotation) * dirToDest.normalized, isFast);
            return false;
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
        
        public void LookAtFixed(Vector3 target)
        {
            var direction = (target - Position).normalized;
            var lookAtRotation = Quaternion.LookRotation(direction, Vector3.up);
            
            Yaw = lookAtRotation.eulerAngles.y;
            Pitch = lookAtRotation.eulerAngles.x;
        }

        // The main camera, through with the user sees the scene
        //[NonSerialized]
        //public Camera MainCamera;
    }
}
