using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.System.Utility;
using UnityEngine;
using Zenject;
using static Assets.Scripts.System.SceneJsonReaderSystem;

namespace Assets.Scripts.Visuals
{
    public class InfiniteGroundVisuals : MonoBehaviour
    {
        //public new Camera camera;

        //Used to change the position of the tiles in the "update()"
        const int tilePositionChange = 16;

        [SerializeField]
        private GameObject groundTemplate;

        [SerializeField]
        private int createDistance = 10;

        [SerializeField]
        private int destroyDistance = 13;

        private Dictionary<Vector2Int, InfiniteGroundTile> tiles = new Dictionary<Vector2Int, InfiniteGroundTile>();

        // Dependencies
        private CameraState cameraState;

        //private IPathState _pathState;
        private SceneJsonReaderSystem sceneJsonReaderSystem;

        [Inject]
        private void Construct( /*IPathState pathState,*/ CameraState cameraState, SceneJsonReaderSystem sceneJsonReaderSystem)
        {
            this.cameraState = cameraState;
            //_pathState = pathState;
            this.sceneJsonReaderSystem = sceneJsonReaderSystem;
        }


        // Update is called once per frame
        void Update()
        {
            DecentralandSceneData decentralandSceneData = sceneJsonReaderSystem.GetSceneData(false);

            var parcelInformation = decentralandSceneData?.GetParcelsInformation();

            parcelInformation ??= new List<DecentralandSceneData.ParcelInformation>();

            var downScaledCameraPosition = cameraState.Position / tilePositionChange;
            var campos = new Vector2Int((int)downScaledCameraPosition.x, (int)downScaledCameraPosition.z);

            // Create new Ground Tile
            var startPos = campos - new Vector2Int(createDistance, createDistance);


            // setup the land positions with the base land in the center at (0, 0)
            List<Vector2Int> availableLandPosition = null;
            if (decentralandSceneData != null)
            {
                availableLandPosition =
                    parcelInformation
                        .Select(parcelInfo => parcelInfo.GetPosition())
                        .Select(pos => pos - decentralandSceneData.GetBaseParcelInformation().GetPosition())
                        .ToList();
            }


            for (var i = 0; i < createDistance * 2 + 1; i++)
            {
                for (var j = 0; j < createDistance * 2 + 1; j++)
                {
                    var currentPos = startPos + new Vector2Int(i, j);

                    if (!tiles.ContainsKey(currentPos))
                    {
                        // +0.5 so that the tile is aligned to the white grid
                        float currentPosMovedX = currentPos.x + 0.5f;
                        float currentPosMovedY = currentPos.y + 0.5f;

                        // Where the tile is located in relationship to the other tiles; 16 because the tile has the size of 16, so that they are not overlapping
                        Vector3 tilePosition =
                            new Vector3(
                                currentPosMovedX * tilePositionChange,
                                0,
                                currentPosMovedY * tilePositionChange);

                        // Create the tile at the calculated position
                        var newTile = Instantiate(groundTemplate, tilePosition, Quaternion.identity, transform).GetComponent<InfiniteGroundTile>();

                        // show grass for the lands, that are *not* part of the scene. When there was an error with scene.json reading, show no grass
                        newTile.ShowDefaultGrass = !availableLandPosition?.Contains(currentPos) ?? false;

                        tiles.Add(currentPos, newTile);

                        //goto endCreateLoop; // create only one tile per frame
                    }
                }
            }

            //endCreateLoop:


            // Delete Tiles that are to far away
            var minPos = campos - new Vector2Int(destroyDistance, destroyDistance);
            var maxPos = campos + new Vector2Int(destroyDistance, destroyDistance);

            List<Vector2Int> keysToRemove = new List<Vector2Int>();

            foreach (var tile in tiles)
            {
                if (tile.Key.x < minPos.x || tile.Key.y < minPos.y || tile.Key.x > maxPos.x || tile.Key.y > maxPos.y)
                {
                    Destroy(tile.Value.gameObject);
                    keysToRemove.Add(tile.Key);
                    //break; // remove only one tile per frame
                }
            }

            foreach (var keyToRemove in keysToRemove)
            {
                tiles.Remove(keyToRemove);
            }
        }
    }
}
