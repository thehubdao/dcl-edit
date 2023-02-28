using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class GizmoSizeVisuals : MonoBehaviour
    {
        // Dependencies
        private GizmoSizeSystem gizmoSizeSystem;
        private CameraState cameraState;

        public GameObject doubleCoords;

        [Inject]
        private void Construct(GizmoSizeSystem gizmoSizeSystem, CameraState cameraState)
        {
            this.gizmoSizeSystem = gizmoSizeSystem;
            this.cameraState = cameraState;
        }

        void LateUpdate()
        {
            var size = gizmoSizeSystem.GetGizmoSize(transform.position);

            transform.localScale = new Vector3(size, size, size);
            doubleCoords.transform.LookAt(cameraState.Position);
        }
    }
}