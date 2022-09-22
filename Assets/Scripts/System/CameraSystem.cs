using Assets.Scripts.EditorState;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Assets.Scripts.System
{
    public class CameraSystem
    {
        public void CameraStartup()
        {
            ChooseReasonableStartPosition();
        }

        public void ChooseReasonableStartPosition()
        {
            // Calculate average parcel center
            var averageCenter = Vector2.zero;
            
            // TODO: look for the used parcels
            //foreach (var parcel in DclSceneManager.sceneJson.scene.Parcels)
            //{
            //    var nulledParcel = parcel - DclSceneManager.sceneJson.scene.Base;
            //    averageCenter += (Vector2)nulledParcel * 16 + new Vector2(8, 8);
            //}
            //
            //if (DclSceneManager.sceneJson.scene.Parcels.Length > 0)
            //    averageCenter /= DclSceneManager.sceneJson.scene.Parcels.Length;
            //
            //var averageCenterWorldPoint = new Vector3(averageCenter.x, 0, averageCenter.y);
            ////Debug.DrawRay(averageCenterWorldPoint,Vector3.up,Color.red,10);
            //
            //Position = averageCenterWorldPoint;

            // Move Camera up
            EditorStates.CurrentCameraState.Yaw = 45;
            EditorStates.CurrentCameraState.Pitch = 30;

            //var dist = Mathf.Log(DclSceneManager.sceneJson.scene.Parcels.Length, 2);

            //if (dist < 0)
            //    dist = 0;

            EditorStates.CurrentCameraState.MoveFixed(new Vector3(0, 0, -10 * (/*dist*/ 1 + 1)));

            EditorStates.CurrentCameraState.Pitch = 45;
        }
    }
}
