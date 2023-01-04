using Assets.Scripts.EditorState;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Zenject;

public class AddRenderTexture : MonoBehaviour
{
    private Camera cam;

    // Dependencies
    private UnityState _unityState;

    [Inject]
    private void Construct(UnityState unityState)
    {
        _unityState = unityState;
    }

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();

    }


    private Vector2Int lastSize = Vector2Int.zero;

    void Update()
    {
        Vector2Int viewPortSize = GetViewPortSize();

        if (lastSize != viewPortSize)
        {
            UpdateRenderTexture(viewPortSize);
        }
    }

    private void UpdateRenderTexture(Vector2Int viewPortSize)
    {
        cam.targetTexture = new RenderTexture(viewPortSize.x, viewPortSize.y, 1, DefaultFormat.LDR);
        cam.forceIntoRenderTexture = true;
        
        lastSize = viewPortSize;

        if (gameObject.name == "Main Camera") // TODO: Make a more robust solution for that
            _unityState.SceneImage.texture = cam.targetTexture;
    }

    private Vector2Int GetViewPortSize()
    {
        Vector3[] fourCorners = new Vector3[4];
        _unityState.SceneImage.rectTransform.GetWorldCorners(fourCorners);
        Vector2Int viewPortSize = new Vector2Int((int)(fourCorners[2].x - fourCorners[0].x), (int)(fourCorners[2].y - fourCorners[0].y));
        return viewPortSize;

    }
}
