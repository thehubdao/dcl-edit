using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    // Allow only one simultaneous generation of thumbnails to avoid models showing up in other models thumbnails.
    private SemaphoreSlim isGenerating = new SemaphoreSlim(1);

    [Inject]
    private void Construct()
    {
        DisableComponents();
    }

    /// <summary>
    /// Generates a preview for the given model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<Texture2D> Generate(GameObject model)
    {
        await isGenerating.WaitAsync();

        GameObject copy = AssetCacheSystem.CreateCopy(model);

        Texture2D thumbnail = await CreateThumbnailOfModel(copy);

        isGenerating.Release();

        return thumbnail;
    }

    private async Task<Texture2D> CreateThumbnailOfModel(GameObject model)
    {
        if (model == null) return null;

        model.transform.SetParent(assetHolder.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        RenderTexture rt = new RenderTexture(thumbnailSize.x, thumbnailSize.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        camera.targetTexture = rt;
        RenderTexture.active = rt;

        // Renderers must be active in hierarchy for the bounds to work
        EnableComponents();
        model.SetActive(true);

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
        texture.Apply();
        RenderTexture.active = null;
        DisableComponents();

        Destroy(model);

        // Wait for the next frame. Generate only one texture per frame to prevent two objects
        // from appearing on one thumbnail.
        await Task.Yield();

        return texture;
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