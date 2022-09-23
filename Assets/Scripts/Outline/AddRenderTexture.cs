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


    private Vector2 lastSize = Vector2.zero;

    void Update()
    {
        if (lastSize != ViewPortSize)
        {
            UpdateRenderTexture();
        }
    }

    private Vector2 ViewPortSize => _unityState.SceneImage.rectTransform.rect.size;


    private void UpdateRenderTexture()
    {
        //Debug.Log("Updating RT to " + ViewPortSize);

        cam.targetTexture = new RenderTexture((int)ViewPortSize.x, (int)ViewPortSize.y, 1, DefaultFormat.LDR);
        cam.forceIntoRenderTexture = true;
        
        lastSize = ViewPortSize;

        if (gameObject.name == "Main Camera") // TODO: Make a more robust solution for that
            _unityState.SceneImage.texture = cam.targetTexture;
    }
}
