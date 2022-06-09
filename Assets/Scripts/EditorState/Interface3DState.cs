using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.EditorState
{
    public class Interface3DState : MonoBehaviour
    {
        private GameObject _currentlyHoveredObject = null;
        public GameObject CurrentlyHoveredObject
        {
            get => _currentlyHoveredObject;
            set
            {
                if (_currentlyHoveredObject != value)
                {
                    _currentlyHoveredObject = value; 
                    HoverChangeEvent.Invoke();
                }
            }
        }


        public readonly UnityEvent HoverChangeEvent = new UnityEvent();
    }
}
