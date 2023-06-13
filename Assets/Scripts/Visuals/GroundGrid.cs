using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Visuals
{
    public class GroundGrid : MonoBehaviour
    {
        private readonly int shaderDistanceRadius = Shader.PropertyToID("_Radius");

        [SerializeField] private Material gridMaterial;

        [SerializeField] private Transform smallGrid;
        [SerializeField] private Transform largeGrid;

        [SerializeField] private int gridSize = 10;

        [SerializeField] private int smallLineCount = 50;
        [SerializeField] private int largeLineCount = 10;
        [SerializeField] private float length = 10000;

        private Material XaxisMaterial;
        private Material ZaxisMaterial;
        private Material smallGridMaterial;
        private Material largeGridMaterial;

        private bool mainCamRender;
        private Camera mainCam;
        private Transform mainCamTransform;

        private int gridHeightScale;
        private Vector3 cameraPos;
        private int gridIndex;

        public void SetGridSize(int value) => gridSize = value > 2 ? value : 2;

        private void Awake()
        {
            mainCam = Camera.main;
            mainCamTransform = mainCam.transform;
            
            smallGridMaterial = new(gridMaterial);
            smallGridMaterial.color = new(0.7f, 0.7f, 0.7f, 1);
            
            largeGridMaterial = new(gridMaterial);
            largeGridMaterial.color = new(1f, 1f, 1f, 1f);

            XaxisMaterial = new(gridMaterial);
            XaxisMaterial.SetFloat(shaderDistanceRadius, 500f);
            XaxisMaterial.color = new(1, 0.2f, 0.2f, 1);

            ZaxisMaterial = new(gridMaterial);
            ZaxisMaterial.SetFloat(shaderDistanceRadius, 500f);
            ZaxisMaterial.color = new(0.2f, 0.2f, 1, 1);
        }

        private void OnEnable() => RenderPipelineManager.beginCameraRendering += WriteLogMessage;
        private void OnDisable() => RenderPipelineManager.beginCameraRendering -= WriteLogMessage;
        private void WriteLogMessage(ScriptableRenderContext context, Camera camera) => mainCamRender = camera == mainCam;

        public void OnRenderObject()
        {
            if (!mainCamRender) return;

            if(cameraPos.y != mainCamTransform.position.y)
            {
                float cameraHeight = Mathf.Abs(mainCamTransform.position.y) - 10;
                gridIndex = cameraHeight > 1 ? Mathf.FloorToInt(Mathf.Log(cameraHeight, gridSize)) : 0;
                gridHeightScale = (int)Mathf.Pow(gridSize, gridIndex);
                smallGridMaterial.SetFloat(shaderDistanceRadius, gridHeightScale);
                largeGridMaterial.SetFloat(shaderDistanceRadius, gridHeightScale);
            }
            cameraPos = mainCamTransform.position;
            
            DrawAxis(ZaxisMaterial, 0f, length);
            DrawAxis(XaxisMaterial, length, 0f);

            DrawGrid(largeGridMaterial, gridHeightScale * gridSize, largeLineCount);
            DrawGrid(smallGridMaterial, gridHeightScale, smallLineCount);
        }

        private Vector3 GetGridPos(float gridHeightScale)
        {
            Vector3 gridPosition = new();
            //Move Incrementaly
            gridPosition.x = Mathf.Floor(cameraPos.x / gridHeightScale) * gridHeightScale;
            gridPosition.z = Mathf.Floor(cameraPos.z / gridHeightScale) * gridHeightScale;
            return gridPosition;
        }

        private void DrawAxis(Material gridMaterial, float x, float z)
        {
            gridMaterial.SetPass(0);
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Vertex3(x, 0f, z);
            GL.Vertex3(-x, 0f, -z);
            GL.End();
            GL.PopMatrix();
        }

        private void DrawGrid(Material gridMaterial, int gridHeightScale, int lineCount)
        {
            Vector3 gridPos = GetGridPos(gridHeightScale);

            int lengthXLeft = Mathf.FloorToInt(gridPos.x - lineCount * gridHeightScale);
            int lengthXRight = Mathf.FloorToInt(gridPos.x + lineCount * gridHeightScale);
            
            int lengthZUp = Mathf.FloorToInt(gridPos.z + lineCount * gridHeightScale);
            int lengthZDown = Mathf.FloorToInt(gridPos.z - lineCount * gridHeightScale);

            gridMaterial.SetPass(0);
            GL.PushMatrix();
            GL.Begin(GL.LINES);

            //< 0 and > 0 => Dont Draw Lines over X | Z main Axis
            for (int i = lengthZDown; i < 0; i += gridHeightScale)
            {
                GL.Vertex3(-length, 0f, i);
                GL.Vertex3(length, 0f, i);
            }

            for (int i = lengthZUp; i > 0; i -= gridHeightScale)
            {
                GL.Vertex3(-length, 0f, i);
                GL.Vertex3(length, 0f, i);
            }

            for (int i = lengthXLeft; i < 0; i += gridHeightScale)
            {
                GL.Vertex3(i, 0f, -length);
                GL.Vertex3(i, 0f, length);
            }

            for (int i = lengthXRight; i > 0; i -= gridHeightScale)
            {
                GL.Vertex3(i, 0f, -length);
                GL.Vertex3(i, 0f, length);
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}