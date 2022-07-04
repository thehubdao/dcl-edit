using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.Visuals
{
    public abstract class Interface3DHover : MonoBehaviour, ISetupSceneEventListeners
    {
        private bool _isHovering = false;

        // TODO: Remove
        void Start()
        {
            SetupSceneEventListeners();
        }
         

        public void SetupSceneEventListeners()
        {
            EditorStates.CurrentInterface3DState.HoverChangeEvent.AddListener(() =>
            {
                if (EditorStates.CurrentInterface3DState.CurrentlyHoveredObject == gameObject)
                {
                    if (!_isHovering)
                    {
                        StartHover();
                        _isHovering = true;
                    }
                    
                    UpdateHover();
                }
                else
                {
                    if (_isHovering)
                    {
                        EndHover();
                        _isHovering = false;
                    }
                }
            });
        }
        
        public virtual void StartHover()
        {

        }

        public virtual void UpdateHover()
        {

        }
        
        public virtual void EndHover()
        {

        }

        
    }
}
