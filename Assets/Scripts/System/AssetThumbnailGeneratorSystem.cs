using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class AssetThumbnailGeneratorSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject assetHolder;

    [SerializeField]
    new private Camera camera;

    [SerializeField]
    private Vector2Int thumbnailSize = new Vector2Int(512, 512);

    // Used for generating thumbnails of models. This variable determines what percentage of the screen the object should fill
    [SerializeField]
    [Range(0, 1)]
    private float screenFillAmount = 0.9f;
    [SerializeField]
    private Vector3 cameraAngle;

    // Dependencies
    AssetThumbnailGeneratorState _state;
    AssetManagerSystem _assetManagerSystem;
    AssetThumbnailManagerSystem _assetThumbnailManagerSystem;
    EditorEvents _editorEvents;

    private bool generatorRunning = false;

    [Inject]
    private void Construct(AssetThumbnailGeneratorState assetThumbnailGeneratorState, AssetManagerSystem assetManagerSystem, AssetThumbnailManagerSystem assetThumbnailManagerSystem, EditorEvents editorEvents)
    {
        _state = assetThumbnailGeneratorState;
        _assetManagerSystem = assetManagerSystem;
        _assetThumbnailManagerSystem = assetThumbnailManagerSystem;
        _editorEvents = editorEvents;

        _editorEvents.onAssetDataUpdatedEvent += OnAssetDataUpdatedCallback;

        DisableComponents();
    }
    public void Enqueue(Guid id)
    {
        if (_state.queuedAssets.Contains(id) || _state.waitingForAssetData.Contains(id))
        {
            return;
        }

        _state.queuedAssets.Enqueue(id);

        if (!generatorRunning)
        {
            StartCoroutine(ThumbnailGeneratorCoroutine());
        }
    }

    IEnumerator ThumbnailGeneratorCoroutine()
    {
        generatorRunning = true;
        while (_state.queuedAssets.Count > 0)
        {
            Guid id = _state.queuedAssets.Dequeue();

            AssetData data = _assetManagerSystem.GetDataById(id);
            if (data.state == AssetData.State.IsLoading)
            {
                _state.waitingForAssetData.Add(id);
            }
            else
            {
                GenerateThumbnail(data);
            }
            yield return null;
        }
        generatorRunning = false;
    }

    void OnAssetDataUpdatedCallback(List<Guid> ids)
    {
        foreach (Guid id in ids)
        {
            if (_state.waitingForAssetData.Contains(id))
            {
                _state.queuedAssets.Enqueue(id);
                _state.waitingForAssetData.Remove(id);

                if (!generatorRunning)
                {
                    StartCoroutine(ThumbnailGeneratorCoroutine());
                }
            }
        }
    }

    void GenerateThumbnail(AssetData data)
    {
        if (data is ModelAssetData modelData)
        {
            var go = modelData.data.gameObject;

            go.transform.SetParent(assetHolder.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            RenderTexture rt = new RenderTexture(thumbnailSize.x, thumbnailSize.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            camera.targetTexture = rt;
            RenderTexture.active = rt;

            // Renderers must be active in hierarchy for the bounds to work
            EnableComponents();
            go.SetActive(true);

            var totalBounds = GetTotalBoundsOfChildren();
            if (totalBounds.HasValue)
            {
                var maxBoundingBoxDistance = (totalBounds.Value.max - totalBounds.Value.min).magnitude;
                camera.transform.position = totalBounds.Value.center;
                camera.nearClipPlane = -maxBoundingBoxDistance;
                camera.transform.rotation = Quaternion.Euler(cameraAngle);
                SetOrthographicCameraSize(totalBounds.Value);
            }

            // Render image from camera
            camera.Render();
            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = null;
            DisableComponents();

            Destroy(go);

            _assetThumbnailManagerSystem.SetThumbnailById(data.id, texture);
        }
        else if (data is ImageAssetData imageData)
        {
            // Just return the texture
            Texture2D thumbnail = imageData.data;
            _assetThumbnailManagerSystem.SetThumbnailById(data.id, thumbnail);
        }
    }

    Bounds? GetTotalBoundsOfChildren()
    {
        var renderers = assetHolder.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) { return null; }
        // Collider must be active in hierarchy for the bounds to work
        foreach (var r in renderers) { r.gameObject.SetActive(true); }
        // The bounds struct must be initialized with the first real collider bounds.
        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            b.Encapsulate(renderers[i].bounds);
        }
        return b;
    }
    List<Vector3> GetCornersOfBounds(Bounds b)
    {
        var corners = new Vector3[8];
        corners[0] = b.min;
        corners[1] = b.max;
        corners[2] = new Vector3(corners[0].x, corners[0].y, corners[1].z);
        corners[3] = new Vector3(corners[0].x, corners[1].y, corners[0].z);
        corners[4] = new Vector3(corners[1].x, corners[0].y, corners[0].z);
        corners[5] = new Vector3(corners[0].x, corners[1].y, corners[1].z);
        corners[6] = new Vector3(corners[1].x, corners[0].y, corners[1].z);
        corners[7] = new Vector3(corners[1].x, corners[1].y, corners[0].z);
        return corners.ToList();
    }

    void SetOrthographicCameraSize(Bounds bounds)
    {
        camera.orthographicSize = 1;

        var corners = GetCornersOfBounds(bounds);

        var minPointScreenSpace = Vector2.positiveInfinity;
        var maxPointScreenSpace = Vector2.negativeInfinity;
        foreach (var c in corners)
        {
            var screenSpacePoint = camera.WorldToScreenPoint(c);
            minPointScreenSpace = Vector2.Min(screenSpacePoint, minPointScreenSpace);
            maxPointScreenSpace = Vector2.Max(screenSpacePoint, maxPointScreenSpace);
        }
        var screenSpaceSize = maxPointScreenSpace - minPointScreenSpace;

        // The bounding box of the asset fills x percent of the screens width/height
        var screenFillPercent = screenSpaceSize / thumbnailSize;

        float newCameraSize;
        if (screenFillPercent.x > screenFillPercent.y)
        {
            newCameraSize = screenFillPercent.x / screenFillAmount;
        }
        else
        {
            newCameraSize = screenFillPercent.y / screenFillAmount;
        }
        camera.orthographicSize = newCameraSize;
    }


    void EnableComponents()
    {
        assetHolder.SetActive(true);
        camera.gameObject.SetActive(true);
    }

    void DisableComponents()
    {
        assetHolder.SetActive(false);
        camera.gameObject.SetActive(false);
    }
}