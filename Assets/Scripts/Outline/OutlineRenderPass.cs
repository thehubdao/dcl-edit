using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

class OutlineRenderPass : ScriptableRenderPass
{
    // used to label this pass in Unity's Frame Debug utility
    string profilerTag;

    Material[] materialsToBlit;
    Material combineMat;
    RenderTargetIdentifier cameraColorTargetIdent;
    RenderTargetHandle tempTexture;
    RenderTargetHandle originalTexture;
    Texture overlayTexture;
    Camera overlayCamera;

    private int rtWidth = -1;
    private int rtHeight = -1;


    public OutlineRenderPass(string profilerTag,
      RenderPassEvent renderPassEvent, Material[] materialsToBlit, Material combineMat, Texture overlayTexture)
    {
        this.profilerTag = profilerTag;
        this.renderPassEvent = renderPassEvent;
        this.materialsToBlit = materialsToBlit;
        this.combineMat = combineMat;
        this.overlayTexture = overlayTexture;

        //this.overlayCamera = GameObject.Find("TextureCam").GetComponent<Camera>();

        //materialToBlit.SetTexture("_overlay", overlayTexture);
    }

    private void resetRenderTexture(Camera camera)
    {
        rtWidth = Screen.width;
        rtHeight = Screen.height;

        RenderTexture rt = new RenderTexture(rtWidth, rtHeight, 24);
        camera.targetTexture = rt; //Create new renderTexture and assign to camera

        //materialToBlit.SetTexture("_overlay", rt);
    }

    // This isn't part of the ScriptableRenderPass class and is our own addition.
    // For this custom pass we need the camera's color target, so that gets passed in.
    public void Setup(RenderTargetIdentifier cameraColorTargetIdent)
    {
        //if (rtWidth != Screen.width || rtHeight != Screen.height)
        //    resetRenderTexture(this.overlayCamera);

        this.cameraColorTargetIdent = cameraColorTargetIdent;
    }

    // called each frame before Execute, use it to set up things the pass will need
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // create a temporary render texture that matches the camera
        cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
        cmd.GetTemporaryRT(3892475, cameraTextureDescriptor);
        //originalTexture = RenderTexture.GetTemporary(cameraTextureDescriptor);

    }

    // Execute is called for every eligible camera every frame. It's not called at the moment that
    // rendering is actually taking place, so don't directly execute rendering commands here.
    // Instead use the methods on ScriptableRenderContext to set up instructions.
    // RenderingData provides a bunch of (not very well documented) information about the scene
    // and what's being rendered.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // fetch a command buffer to use
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
        cmd.Clear();

        cmd.Blit(cameraColorTargetIdent, 3892475);

        for (int i = 0; i < materialsToBlit.Length; i++)
        {
            var src = i % 2 == 0 ? cameraColorTargetIdent : tempTexture.Identifier();
            var dst = i % 2 == 0 ? tempTexture.Identifier() : cameraColorTargetIdent;
            cmd.Blit(src, dst, materialsToBlit[i], 0);
        }

        if (materialsToBlit.Length % 2 == 0)
        {
            cmd.Blit(cameraColorTargetIdent, tempTexture.Identifier());
        }

        cmd.SetGlobalTexture("_overlay", 3892475);

        //combineMat.SetTexture("_overlay", originalTexture);

        cmd.Blit(tempTexture.Identifier(), cameraColorTargetIdent, combineMat, 0);

        // don't forget to tell ScriptableRenderContext to actually execute the commands
        context.ExecuteCommandBuffer(cmd);

        // tidy up after ourselves
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    // called after Execute, use it to clean up anything allocated in Configure
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tempTexture.id);
        cmd.ReleaseTemporaryRT(3892475);
    }
}
