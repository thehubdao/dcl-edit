using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Assets;
using UnityEngine;
using Zenject;
using static AssetThumbnailGeneratorState;

public class AssetThumbnailGeneratorSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject assetHolder;
    
    [SerializeField]
    private new Camera camera;
    
    [SerializeField]
    private Vector2Int thumbnailSize = new Vector2Int(512, 512);
    
    // Used for generating thumbnails of models. This variable determines what percentage of the screen the object should fill
    [SerializeField]
    [Range(0, 1)]
    private float screenFillAmount = 0.9f;
    [SerializeField]
    private Vector3 cameraAngle;
    
    // Dependencies
    AssetThumbnailGeneratorState state;
    //AssetThumbnailManagerSystem assetThumbnailManagerSystem;
    EditorEvents editorEvents;
    
    private bool generatorRunning = false;
    
    [Inject]
    private void Construct(AssetThumbnailGeneratorState assetThumbnailGeneratorState, /*AssetThumbnailManagerSystem assetThumbnailManagerSystem,*/ EditorEvents editorEvents)
    {
        state = assetThumbnailGeneratorState;
        //this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;
        this.editorEvents = editorEvents;
    
        DisableComponents();
    }
    
    public void GenerateFromModel(CommonAssetTypes.IModelProvider model, Action<Sprite> then)
    {
        state.queuedAssets.Enqueue(new QueuedAsset(model, then));
    
        if (!generatorRunning)
        {
            StartCoroutine(ThumbnailGeneratorCoroutine());
        }
    }

    private IEnumerator ThumbnailGeneratorCoroutine()
    {
        generatorRunning = true;
        while (state.queuedAssets.Count > 0)
        {
            QueuedAsset qa = state.queuedAssets.Dequeue();

            try
            {
                var texture = GenerateThumbnail(qa.model);
                var sprite = Sprite.Create(
                    texture,
                    Rect.MinMaxRect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                qa.then(sprite);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                qa.then(null);
            }
            
            yield return null;
        }
        generatorRunning = false;
    }
    
    private Texture2D GenerateThumbnail(CommonAssetTypes.IModelProvider data)
    {
        //Debug.Log($"Generating Thumbnail: {data.id}");
        
        var goInstance = data.CreateInstance();
        var go = goInstance.gameObject;

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
        var texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        DisableComponents();
    
        goInstance.ReturnToPool();

        return texture;
    }

    private Bounds? GetTotalBoundsOfChildren()
    {
        var rendererInChildren = assetHolder.GetComponentsInChildren<Renderer>();
        if (rendererInChildren.Length == 0) { return null; }
        // Collider must be active in hierarchy for the bounds to work
        foreach (var r in rendererInChildren) { r.gameObject.SetActive(true); }
        // The bounds struct must be initialized with the first real collider bounds.
        var b = rendererInChildren[0].bounds;
        for (var i = 1; i < rendererInChildren.Length; i++)
        {
            b.Encapsulate(rendererInChildren[i].bounds);
        }
        return b;
    }

    private static List<Vector3> GetCornersOfBounds(Bounds b)
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

    private void SetOrthographicCameraSize(Bounds bounds)
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


    private void EnableComponents()
    {
        assetHolder.SetActive(true);
        camera.gameObject.SetActive(true);
    }

    private void DisableComponents()
    {
        assetHolder.SetActive(false);
        camera.gameObject.SetActive(false);
    }
}