using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class NumberPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public NumberInputHandler numberInput;
    }
}
