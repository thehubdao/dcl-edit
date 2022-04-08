using UnityEngine;

namespace Assets.Scripts.System.Utility
{
    public class InfiniteGroundTile : MonoBehaviour
    {
        public GameObject DefaultGrass;


        private bool _showDefaultGrass = true;

        public bool ShowDefaultGrass
        {
            get => _showDefaultGrass;
            set
            {
                _showDefaultGrass = value;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            DefaultGrass.SetActive(ShowDefaultGrass);
        }
    }
}