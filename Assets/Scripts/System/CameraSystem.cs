using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class CameraSystem
    {
        // Dependencies
        private CameraState cameraState;
        private SceneJsonReaderSystem sceneJsonReaderSystem;

        [Inject]
        private void Construct(CameraState cameraState, SceneJsonReaderSystem sceneJsonReaderSystem)
        {
            this.cameraState = cameraState;
            this.sceneJsonReaderSystem = sceneJsonReaderSystem;
        }

        public void CameraStartup()
        {
            ChooseReasonableStartPosition();
        }

        public void ChooseReasonableStartPosition()
        {
            var decentralandSceneData = sceneJsonReaderSystem.GetSceneData(false);
            var baseParcelPosition = decentralandSceneData?.GetBaseParcelInformation().GetPosition() ?? new Vector2Int(0, 0);
            var parcels = decentralandSceneData?.GetParcelsInformation()
                                        .Select(p => p.GetPosition())
                                        .ToList()
                                    ?? new List<Vector2Int> {new Vector2Int(0, 0)};

            var parcelBounds = new Bounds(Vector2.zero, Vector2.zero);
            
            foreach (var parcel in parcels)
            {
                var local = parcel - baseParcelPosition;
                var toAdd = new Vector3(local.x, 0, local.y) * 16 + new Vector3(16, 0, 16);
                parcelBounds.Encapsulate(toAdd);
            }
            
            // Debug.DrawRay(Vector3.zero, parcelBounds.center, Color.magenta, 600);
            // Debug.DrawRay(parcelBounds.center, Vector3.up, Color.red, 600);

            // Yaw to averageCenter from origin (it works since plot location is on plain x,z)
            var newAngle = Vector2.Angle(Vector2.up, new Vector2(parcelBounds.center.x, parcelBounds.center.z));
            
            // Move Camera up
            cameraState.Pitch = 30;
            cameraState.Yaw = newAngle;

            cameraState.Position = parcelBounds.center;
            cameraState.MoveFixed(new Vector3(0, 0, -1 * parcelBounds.size.magnitude));
        }
    }
}
