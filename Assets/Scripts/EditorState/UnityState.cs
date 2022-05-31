using UnityEngine;
using UnityGLTF;

namespace Assets.Scripts.EditorState
{
    public class UnityState : MonoBehaviour
    {
        [SerializeField]
        public GameObject SceneVisuals;

        [SerializeField] 
        public AsyncCoroutineHelper AsyncCoroutineHelper;
    }
} 
