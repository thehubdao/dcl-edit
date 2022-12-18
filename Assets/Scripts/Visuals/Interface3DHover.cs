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
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(Interface3DState interface3DState, EditorEvents editorEvents)
        {
            _interface3DState = interface3DState;
            _editorEvents = editorEvents;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            _editorEvents.onHoverChangedEvent += () =>
            {
                if (_interface3DState.CurrentlyHoveredObject == gameObject)
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
            };
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
