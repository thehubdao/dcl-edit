using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Ookii.Dialogs;
using UnityEngine;


namespace Assets.Scripts.EditorState
{
    public class CameraState
    {
        private float CameraNormalFlySpeed => PersistentData.CameraSpeed;
        private float CameraFastFlySpeed => CameraNormalFlySpeed * 3;
        private float MouseSensitivity => PersistentData.MouseSensitivity;

        private Subscribable<Vector3> _position = new();
        private Subscribable<float> _pitch = new();
        private Subscribable<float> _yaw = new();

        public void AddOnPitchChanged(System.Action action) => _pitch.OnValueChanged += action;
        public void RemoveOnPitchChanged(System.Action action) => _pitch.OnValueChanged -= action;
        
        public float Pitch
        {
            get => _pitch.Value;
            set => _pitch.Value = Mathf.Clamp(value, -100f, 100f);
        }
        public Subscribable<Vector3> Position => _position;
        public Subscribable<float> Yaw => _yaw;

        public Quaternion Rotation => Quaternion.Euler(_pitch.Value, _yaw.Value, 0);
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
            Yaw.Value += directionX * MouseSensitivity;
            Pitch += directionY * -MouseSensitivity;
        }

        public void MoveContinuously(Vector3 localDirection, bool isFast)
        {
            MoveStep(localDirection * Time.deltaTime, isFast);
        }

        public void MoveStep(Vector3 localDirection, bool isFast)
        {
            Position.Value += Rotation * localDirection * (isFast ? CameraFastFlySpeed : CameraNormalFlySpeed);
        }
        public void MoveFixed(Vector3 localDirection)
        {
            Position.Value += Rotation * localDirection;
        }
        /// <summary>
        /// Moves the camera towards the given destination. Returns true if the destination was reached.
        /// </summary>
        public bool MoveTowards(Vector3 globalDestination, bool isFast)
        {
            Vector3 dirToDest = globalDestination - Position.Value;
            Vector3 move = dirToDest.normalized * Time.deltaTime * (isFast ? CameraFastFlySpeed : CameraNormalFlySpeed);

            // Check if destination was reached
            if (dirToDest.magnitude < move.magnitude)
            {
                Position.Value += dirToDest;
                return true;
            }

            MoveContinuously(Quaternion.Inverse(Rotation) * dirToDest.normalized, isFast);
            return false;
        }

        public void RotateAroundPointStep(Vector3 point, Vector2 direction)
        {
            // Find relations from position to Pivot point
            var gimbalVector = Position.Value - point;
            var moveToPivot = Quaternion.Inverse(Rotation) * -gimbalVector;

            // Move to pivot
            MoveFixed(moveToPivot);

            // Rotate
            Yaw.Value += direction.x * MouseSensitivity;
            Pitch += direction.y - MouseSensitivity;

            // Move back from pivot
            MoveFixed(-moveToPivot);
        }

        // The main camera, through with the user sees the scene
        //[NonSerialized]
        //public Camera MainCamera;
    }
}
