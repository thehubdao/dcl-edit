using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public abstract class Interface3DHover : MonoBehaviour
    {
        private bool _isHovering = false;

        // Dependencies
        private Interface3DState _interface3DState;
        
        [Inject]
        private void Construct(Interface3DState interface3DState)
        {
            _interface3DState = interface3DState;
        }

        private void OnEnable()
        {
            _interface3DState.CurrentlyHoveredObject.OnValueChanged += UpdateVisuals;
            UpdateVisuals();
        }

        private void OnDisable()
        {
            _interface3DState.CurrentlyHoveredObject.OnValueChanged -= UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            if (_interface3DState.CurrentlyHoveredObject.Value == gameObject)
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
