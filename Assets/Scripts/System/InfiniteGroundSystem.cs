using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using Assets.Scripts.EditorState;
using Assets.Scripts.System.Utility;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;
using static SceneJsonReader;

namespace Assets.Scripts.System
{
    public class InfiniteGroundSystem : MonoBehaviour
    {
        //public new Camera camera;

        //Used to change the position of the tiles in the "update()"
        const int tilePositionChange = 16;

        [SerializeField]
        private GameObject _groundTemplate;

        [SerializeField]
        private int _createDistance = 10;

        [SerializeField]
        private int _destroyDistance = 13;

        /// <summary>
        /// /Indicates the Parcel coordinates in the middle of the scene at start from where the parcels of the user are built. Used ParcelInformation, because is checked later if it is null
        /// </summary>
        private static DecentralandSceneData.ParcelInformation bottomLeftCornerStartParcelPosition;

        private ObservableCollection<Vector2Int> noGrassTiles = new ObservableCollection<Vector2Int>();


        private Dictionary<Vector2Int, InfiniteGroundTile> _tiles = new Dictionary<Vector2Int, InfiniteGroundTile>();

        // Dependencies
        private CameraState _cameraState;
        private IPathState _pathState;
        private SceneJsonReader sceneJsonReader;

        [Inject]
        private void Construct(IPathState pathState, CameraState cameraState, SceneJsonReader sceneJsonReader)
        {
            _cameraState = cameraState;
            _pathState = pathState;
            this.sceneJsonReader = sceneJsonReader;
        }


        //private Vector3 CameraPositionOnGround => camera.transform.position.OnGroundPlane();

        void Start()
        {
            //foreach (var sceneParcel in DclSceneManager.sceneJson.scene.Parcels)
            //{
            //    noGrassTiles.Add(sceneParcel - DclSceneManager.sceneJson.scene.Base);
            //}

            UpdateGrassShowing();
        }

        public void UpdateGrassShowing()
        {
            foreach (var t in _tiles)
            {
                t.Value.ShowDefaultGrass = !noGrassTiles.Contains(t.Key);
            }
        }



        // Update is called once per frame
        void Update()
        {
            List<DecentralandSceneData.ParcelInformation> parcelInformation = new List<DecentralandSceneData.ParcelInformation>();

            DecentralandSceneData decentralandSceneData = sceneJsonReader.getSceneData(false);

            if (decentralandSceneData != null)
            {
                parcelInformation = sceneJsonReader.getSceneData(false).GetParcelInformation();
            }
            else
            {
                Debug.LogWarning("Error in " + this.GetType().FullName + " because could not read DecentralandSceneData and display the parcels from the user");
            }

            var downScaledCameraPosition = _cameraState.Position / tilePositionChange;
            var campos = new Vector2Int((int)downScaledCameraPosition.x, (int)downScaledCameraPosition.z);

            // Create new Ground Tile
            var startPos = campos - new Vector2Int(_createDistance, _createDistance);

            //Checks for null, because it is set just once at the beginning
            if (bottomLeftCornerStartParcelPosition == null)
            {
                //Takes campos, because it indicates the middle of the scene, because the user starts in the middle
                bottomLeftCornerStartParcelPosition = new DecentralandSceneData.ParcelInformation(((Vector2Int)campos).x, ((Vector2Int)campos).y);
            }

            for (var i = 0; i < _createDistance * 2 + 1; i++)
            {
                for (var j = 0; j < _createDistance * 2 + 1; j++)
                {
                    var currentPos = startPos + new Vector2Int(i, j);

                    if (!_tiles.ContainsKey(currentPos))
                    {
                        //+0.5 so that the tile is aligned to the white grid
                        float currentPosMovedX = currentPos.x + 0.5f;
                        float currentPosMovedY = currentPos.y + 0.5f;

                        //Where the tile is located in relationship to the other tiles; 16 because the tile has the size of 16, so that they are not overlapping
                        Vector3 tilePosition = new Vector3(currentPosMovedX * tilePositionChange, 0,
                            currentPosMovedY * tilePositionChange);

                        Boolean showGrass = true;

                        if (parcelInformation != null)
                        {
                            foreach (DecentralandSceneData.ParcelInformation parcel in parcelInformation)
                            {
                                //Checks if the x currentPos is aligned with the parcel of the user. Is checked in relationship to the bottomLeftCornerStartParcelPosition, because it indicates the start where it starts to build the parcels of the user
                                if (currentPos.x == bottomLeftCornerStartParcelPosition.GetX() + parcel.GetX())
                                {
                                    //Checks afterwards the position on the other 2D axis
                                    if (currentPos.y == bottomLeftCornerStartParcelPosition.GetZ() + parcel.GetZ())
                                    {
                                        //Doesn't show the grass, because underneath the grass is a blue tile and it indicates that the user has his parcel there
                                        showGrass = false;
                                    }
                                }
                            }
                        }

                        var newTile = Instantiate(_groundTemplate, tilePosition, Quaternion.identity, transform).GetComponent<InfiniteGroundTile>();

                        if (showGrass == true)
                        {
                            newTile.ShowDefaultGrass = !noGrassTiles.Contains(currentPos);
                        }
                        else
                        {
                            newTile.ShowDefaultGrass = false;
                        }

                        _tiles.Add(currentPos, newTile);

                        //goto endCreateLoop; // create only one tile per frame
                    }
                }
            }

            //endCreateLoop:


            // Delete Tiles that are to far away
            var minPos = campos - new Vector2Int(_destroyDistance, _destroyDistance);
            var maxPos = campos + new Vector2Int(_destroyDistance, _destroyDistance);

            Vector2Int? keyToRemove = null;

            foreach (var tile in _tiles)
            {
                if (tile.Key.x < minPos.x || tile.Key.y < minPos.y || tile.Key.x > maxPos.x || tile.Key.y > maxPos.y)
                {
                    Destroy(tile.Value.gameObject);
                    keyToRemove = tile.Key;
                    break; // remove only one tile per frame
                }
            }

            if (keyToRemove != null)
                _tiles.Remove(keyToRemove.Value);
        }

    }
}
