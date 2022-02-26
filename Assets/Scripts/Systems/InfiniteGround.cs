using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class InfiniteGround : MonoBehaviour
{
    public new Camera camera;

    public GameObject groundTemplate;

    public int createDistance = 10;
    public int destroyDistance = 13;

    private ObservableCollection<Vector2Int> noGrassTiles = new ObservableCollection<Vector2Int>();


    private Dictionary<Vector2Int, InfiniteGroundTile> _tiles = new Dictionary<Vector2Int, InfiniteGroundTile>();


    private Vector3 CameraPositionOnGround => camera.transform.position.OnGroundPlane();

    void Start()
    {
        foreach (var sceneParcel in DclSceneManager.sceneJson.scene.Parcels)
        {
            noGrassTiles.Add(sceneParcel - DclSceneManager.sceneJson.scene.Base);
        }

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
        var campos = CameraPositionOnGround.ToLandIndex();

        // Create new stuff
        var startPos = campos - new Vector2Int(createDistance, createDistance);
        for (int i = 0; i < createDistance * 2 + 1; i++)
        {
            for (int j = 0; j < createDistance * 2 + 1; j++)
            {
                var currentPos = startPos + new Vector2Int(i, j);
                if (!_tiles.ContainsKey(currentPos))
                {
                    var newTile = Instantiate(groundTemplate, currentPos.ToWorldCoordinates(), Quaternion.identity, transform).GetComponent<InfiniteGroundTile>();
                    newTile.ShowDefaultGrass = !noGrassTiles.Contains(currentPos);
                    _tiles.Add(currentPos, newTile);

                    //goto endCreateLoop; // create only one tile per frame
                }
            }
        }
        //endCreateLoop:


        // Delete Tiles that are to far away
        var minPos = campos - new Vector2Int(destroyDistance, destroyDistance);
        var maxPos = campos + new Vector2Int(destroyDistance, destroyDistance);

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
