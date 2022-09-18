using System;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class EditorStates : MonoBehaviour
    {
        // Current instance
        public static EditorStates Instance;


        // States
        // Camera State
        //[SerializeField]
        //public CameraState CameraState;

        // Scene State
        //[SerializeField]
        //public SceneState SceneState;

        private SceneState _sceneState;

        public void NewSceneState(DclScene scene)
        {
            _sceneState = new SceneState
            {
                CurrentScene = scene
            };
        }

        // Project State
        [SerializeField]
        public ProjectState ProjectState;

        // Path State 
        [SerializeField]
        public PathState PathState;

        // Unity State
        [SerializeField]
        public UnityState UnityState;

        // Model Cache State
        [SerializeField]
        public ModelCacheState ModelCacheState;

        // Interface 3D State
        [SerializeField]
        public Interface3DState Interface3DState;

        [SerializeField]
        public GizmoState GizmoState;

        // Scene dependent editor states
        [Serializable]
        private struct SceneDependentState
        {
            [SerializeField]
            public CameraState CameraState;
        }

        private Dictionary<SceneState, SceneDependentState> _sceneDependentStates = new Dictionary<SceneState, SceneDependentState>();

        private static SceneDependentState? currentSceneDependentState
        {
            get
            {
                if (CurrentSceneState == null)
                    return null;

                if (!Instance._sceneDependentStates.ContainsKey(CurrentSceneState))
                {
                    var sceneDependentState = new SceneDependentState { CameraState = new CameraState() };
                    Instance._sceneDependentStates.Add(CurrentSceneState, sceneDependentState);
                    return sceneDependentState;
                }

                return Instance._sceneDependentStates[CurrentSceneState];
            }
        }

        // Static references to the current states
        public static CameraState CurrentCameraState => currentSceneDependentState?.CameraState;
        public static SceneState CurrentSceneState => Instance._sceneState;
        public static ProjectState CurrentProjectState => Instance.ProjectState;
        public static PathState CurrentPathState => Instance.PathState;
        public static UnityState CurrentUnityState => Instance?.UnityState;
        public static ModelCacheState CurrentModelCacheState => Instance.ModelCacheState;
        public static Interface3DState CurrentInterface3DState => Instance.Interface3DState;
        public static GizmoState CurrentGizmoState => Instance.GizmoState;
    }
}
