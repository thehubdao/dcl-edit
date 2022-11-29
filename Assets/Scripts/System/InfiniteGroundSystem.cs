using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Assets.Scripts.EditorState;
using Assets.Scripts.System.Utility;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

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

        private ObservableCollection<Vector2Int> noGrassTiles = new ObservableCollection<Vector2Int>();


        private Dictionary<Vector2Int, InfiniteGroundTile> _tiles = new Dictionary<Vector2Int, InfiniteGroundTile>();

        // Dependencies
        private CameraState _cameraState;
        private IPathState _pathState;

        [Inject]
        private void Construct(IPathState pathState, CameraState cameraState)
        {
            _cameraState = cameraState;
            _pathState = pathState;
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
            var downScaledCameraPosition = _cameraState.Position / tilePositionChange;
            var campos = new Vector2Int((int)downScaledCameraPosition.x, (int)downScaledCameraPosition.z);

            // Create new Ground Tile
            var startPos = campos - new Vector2Int(_createDistance, _createDistance);
            for (var i = 0; i < _createDistance * 2 + 1; i++)
            {
                for (var j = 0; j < _createDistance * 2 + 1; j++)
                {
                    var currentPos = startPos + new Vector2Int(i, j);
                    if (!_tiles.ContainsKey(currentPos))
                    {
                        //Where the tile is located in relationship to the other tiles; 16 because the tile has the size of 16, so that they are not overlapping; +0.5 so that the tile is aligned to the white grid
                        Vector3 tilePosition = new Vector3((currentPos.x + 0.5f) * tilePositionChange, 0, (currentPos.y + 0.5f) * tilePositionChange);

                        var newTile = Instantiate(_groundTemplate, tilePosition, Quaternion.identity, transform).GetComponent<InfiniteGroundTile>();
                        newTile.ShowDefaultGrass = !noGrassTiles.Contains(currentPos);
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
